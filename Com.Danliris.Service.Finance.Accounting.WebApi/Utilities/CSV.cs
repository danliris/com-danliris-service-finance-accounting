using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Utilities
{
    public static class CSV<TVModel>
        where TVModel : BaseViewModel
    {

        public static IEnumerable<TVModel> ReadFile(IFormFile file)
        {
            using (TextReader reader = new StreamReader(file.OpenReadStream()))
            {
                var csvReader = new CsvReader(reader);
                csvReader.Configuration.HasHeaderRecord = false;

                while (csvReader.Read())
                {
                    yield return csvReader.GetRecord<TVModel>();
                }
            }
        }

    }
}
