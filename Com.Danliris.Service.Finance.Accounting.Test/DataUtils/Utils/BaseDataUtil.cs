using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Moonlay.Models;
using System;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Utils
{
    public abstract class BaseDataUtil<TService, TModel>
        where TService : IBaseService<TModel>
        where TModel : StandardEntity
    {
        private TService _service;
        public BaseDataUtil(TService service)
        {
            _service = service;
        }

        public virtual Task<TModel> GetNewData()
        {
            return Task.FromResult(Activator.CreateInstance(typeof(TModel)) as TModel);
        }

        public virtual async Task<TModel> GetTestData(TModel model = null)
        {
            var data = model ?? await GetNewData();
            await _service.CreateAsync(data);
            return data;
        }
    }
}
