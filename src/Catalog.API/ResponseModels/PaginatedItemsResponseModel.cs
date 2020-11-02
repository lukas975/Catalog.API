using System.Collections.Generic;

namespace Catalog.API.ResponseModels
{
    public class PaginatedItemsResponseModel <TEntity> where TEntity : class
    {
        public PaginatedItemsResponseModel(int pageIndex, int pageSize, long total, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Total = total;
            Data = data;
        }

        public int PageIndex { get; }
        public int PageSize { get; }
        public long Total { get; }
        public IEnumerable<TEntity> Data { get; }
    }
}
