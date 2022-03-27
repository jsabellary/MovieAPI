using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Mapping;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieAPI.Controllers
{
    [ApiController]
    public class moviesController : ControllerBase
    {
        // GET: /movies/stats
        [HttpGet]
        [Route("movies/stats")]
        public IEnumerable<statsResult> Get()
        {

            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvstatsMapping csvMapper = new CsvstatsMapping();
            CsvParser<stats> csvParser = new CsvParser<stats>(csvParserOptions, csvMapper);
            var result = csvParser.ReadFromFile(@"stats.csv", Encoding.ASCII);

            List<stats> result1 = result.Select(x => x.Result).ToList();

            var result2 = result1
                            .GroupBy(x => x.movieId)
                            .Select(rl1 => new statsResult
                            {
                                movieId = rl1.First().movieId,
                                averageWatchDurationS = (int)rl1.Average(c => c.watchDurationMs),
                                watches = rl1.Count()
                            }).ToList();

            foreach (statsResult statsResult in result2.ToList())
            {
                metadata retrived = retrive(statsResult.movieId);
                if (retrived != null)
                {
                    statsResult.title = retrived.title;
                    statsResult.releaseYear = retrived.releaseYear;
                }
                else
                {
                    result2.Remove(statsResult);
                }
            }
            result2 = result2.OrderByDescending(x => x.averageWatchDurationS).ToList();
            return result2;
        }

        public class statsResult
        {
            public statsResult() { }
            public int movieId { get; set; }
            public string title { get; set; }
            public int averageWatchDurationS { get; set; }
            public int watches { get; set; }
            public int releaseYear { get; set; }
        }

        private class CsvstatsMapping : CsvMapping<stats>
        {
            public CsvstatsMapping()
                : base()
            {
                MapProperty(0, x => x.movieId);
                MapProperty(1, x => x.watchDurationMs);
            }
        }

        private class CsvmetadataMapping : CsvMapping<metadata>
        {
            public CsvmetadataMapping()
                : base()
            {
                MapProperty(0, x => x.Id);
                MapProperty(1, x => x.movieId);
                MapProperty(2, x => x.title);
                MapProperty(3, x => x.language);
                MapProperty(4, x => x.duration);
                MapProperty(5, x => x.releaseYear);
            }
        }

        private metadata retrive(int id)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvmetadataMapping csvMapper = new CsvmetadataMapping();
            CsvParser<metadata> csvParser = new CsvParser<metadata>(csvParserOptions, csvMapper);
            var result = csvParser.ReadFromFile(@"metadata.csv", Encoding.ASCII);

            List<metadata> result1 = result.Select(x => x.Result).Where(x => x.movieId == id).ToList();

            if (result1.Count == 0) { return null; }
            metadata result2 = result1.OrderByDescending(x => x.releaseYear).First();

            return result2;
        }
    }
}
