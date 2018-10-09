using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface
{
    public interface IBaseLogic<TModel>
    {
        void CreateModel(TModel model);
        Task<TModel> ReadModelById(int id);
        void UpdateModelAsync(int id, TModel model);
        Task DeleteModel(int id);
    }
}
