using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieAPI.Controllers
{
    [ApiController]
    public class metadataController : ControllerBase
    {
        public List<metadata> database = new List<metadata>();

        // Post /metadata
        [HttpPost]
        [Route("metadata")]
        public void Post([FromBody] metadata data)
        {
            database.Add(data);
        }

        // GET /metadata/5
        [HttpGet]
        [Route("metadata/{id}")]
        public IEnumerable<metadataResult> Get(int id)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvmetadataMapping csvMapper = new CsvmetadataMapping();
            CsvParser<metadata> csvParser = new CsvParser<metadata>(csvParserOptions, csvMapper);
            var result = csvParser.ReadFromFile(@"metadata.csv", Encoding.ASCII);

            List<metadata> result1 = result.Select(x => x.Result).Where(x => x.movieId == id).ToList();

            if (result1.Count == 0)
            {
                return null;
            }

            result1 = result1
                             .GroupBy(y => y.language)
                             .Select(g => g.OrderByDescending(z => z.Id).First())
                             .OrderBy(g => g.language)
                             .ToList();
            var result2 = result1.Select(x => new metadataResult
            {
                movieId = x.movieId,
                title = x.title,
                language = x.language,
                duration = x.duration,
                releaseYear = x.releaseYear,
            }).ToList();

            return result2;
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

        public class metadataResult
        {
            public metadataResult() { }
            public int movieId { get; set; }
            public string title { get; set; }
            public string language { get; set; }
            public string duration { get; set; }
            public int releaseYear { get; set; }
        }
    }
}
