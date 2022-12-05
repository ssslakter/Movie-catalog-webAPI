using MovieCatalogAPI.Models.DTO;

namespace MovieCatalogAPI.Models
{
    public class MoviesPagedListModel
    {
        public PageInfoModel PageInfo { get; set; }
        public IEnumerable<MovieElementModel> Movies { get; set; }        

        public MoviesPagedListModel(PageInfoModel pageInfo)
        {
            PageInfo = pageInfo;
        }
    }
}
