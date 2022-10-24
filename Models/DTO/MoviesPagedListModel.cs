using MovieCatalogAPI.Models.DTO;

namespace MovieCatalogAPI.Models
{
    public class MoviesPagedListModel
    {
        public MovieElementModel[]? movies { get; set; }
        public PageInfoModel PageInfo { get; set; }

        public MoviesPagedListModel(PageInfoModel pageInfo)
        {
            PageInfo = pageInfo;
        }
    }
}
