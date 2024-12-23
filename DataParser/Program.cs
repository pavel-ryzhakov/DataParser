using System.Globalization;
using System.Text;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using static System.Formats.Asn1.AsnWriter;

namespace DataParser
{
    internal class Program
    {
        private const int BatchSize = 100000;

        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    var connectionString = ctx.Configuration.GetConnectionString("DefaultConnection");
                    var csvFilePath = ctx.Configuration["CsvFilePath"];
                    services.AddDbContextFactory<PassportsDbContext>(options =>
                        options.UseNpgsql(connectionString));
                    services.AddSingleton(new CsvConfiguration
                    {
                        FilePath = csvFilePath,
                        ConnectionString = connectionString
                    });

                });

            var app = builder.Build();
            var csvFilePath = app.Services.GetRequiredService<CsvConfiguration>().FilePath;
            var connectionString = app.Services.GetRequiredService<CsvConfiguration>().ConnectionString;


            Console.WriteLine("Migration starting.");
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PassportsDbContext>();
                context.Database.Migrate();

            }

            Console.WriteLine("Migration success");


            var readCsvBlock = new TransformManyBlock<string, PassportRow>(
                async filePath => await ReadAndValidateCsvFileAsync(filePath),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 8,
                    BoundedCapacity = 8 
                }
            );

            var batchBlock = new BatchBlock<PassportRow>(BatchSize);

            var writeToDatabaseBlock = new ActionBlock<PassportRow[]>(
                async batch => await WriteBatchToDatabaseAsync(batch, connectionString),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 20, 
                    BoundedCapacity = 40
                }
            );

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            readCsvBlock.LinkTo(batchBlock, linkOptions);
            batchBlock.LinkTo(writeToDatabaseBlock, linkOptions);

            readCsvBlock.Post(csvFilePath);
            readCsvBlock.Complete();

            await writeToDatabaseBlock.Completion;
        }

        private static async Task<IEnumerable<PassportRow>> ReadAndValidateCsvFileAsync(string filePath)
        {
            var validRows = new List<PassportRow>();

            await using var fileStream =
                new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true);
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<PassportRowMap>();

            await foreach (var record in csv.GetRecordsAsync<PassportRow>())
            {

                if (!short.TryParse(record.PASSP_SERIES_RAW, out var series) ||
                    record.PASSP_SERIES_RAW.Trim().Length != 4)
                    continue;
                if (!int.TryParse(record.PASSP_NUMBER_RAW, out var number) ||
                    record.PASSP_NUMBER_RAW.Trim().Length != 6)
                    continue;

                validRows.Add(new PassportRow
                {
                    PASSP_SERIES = series,
                    PASSP_NUMBER = number
                });
            }

            return validRows;
        }

        private static async Task WriteBatchToDatabaseAsync(PassportRow[] batch,string connectionString)
        {
            using var connectionForSeries =
                new NpgsqlConnection(connectionString);
            using var connectionForBinaryImport =
                new NpgsqlConnection(connectionString);

            await connectionForSeries.OpenAsync();
            await connectionForBinaryImport.OpenAsync();

            try
            {
 
                var seriesLookup = await GetOrInsertSeriesAsync(connectionForSeries, batch);

                await using var writer = connectionForBinaryImport.BeginBinaryImport(
                    "COPY passport_numbers (series_id, number, status) FROM STDIN (FORMAT BINARY)");

                foreach (var row in batch)
                {
                    writer.StartRow();
                    writer.Write(seriesLookup[row.PASSP_SERIES], NpgsqlTypes.NpgsqlDbType.Integer);
                    writer.Write(row.PASSP_NUMBER, NpgsqlTypes.NpgsqlDbType.Integer);
                    writer.Write(false, NpgsqlTypes.NpgsqlDbType.Boolean); 
                }

                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing batch: {ex.Message}");
                throw;
            }
        }

        private static async Task<Dictionary<short, int>> GetOrInsertSeriesAsync(NpgsqlConnection connection,
            PassportRow[] batch)
        {
            var seriesLookup = new Dictionary<short, int>();
            var distinctSeries = batch.Select(row => row.PASSP_SERIES).Distinct();

            foreach (var series in distinctSeries)
            {
                await using var selectCommand = new NpgsqlCommand(
                    "SELECT series_id FROM passport_series WHERE series = @series", connection);
                selectCommand.Parameters.AddWithValue("series", series);

                var result = await selectCommand.ExecuteScalarAsync();
                if (result != null)
                {
                    seriesLookup[series] = (int)result;
                    continue;
                }

                await using var insertCommand = new NpgsqlCommand(
                    "INSERT INTO passport_series (series) VALUES (@series) RETURNING series_id", connection);
                insertCommand.Parameters.AddWithValue("series", series);

                var seriesId = (int)(await insertCommand.ExecuteScalarAsync());
                seriesLookup[series] = seriesId;
            }

            return seriesLookup;
        }
    }
}



