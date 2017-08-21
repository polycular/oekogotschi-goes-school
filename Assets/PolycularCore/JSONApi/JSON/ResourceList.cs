using System.Collections.Generic;

namespace JSONApi.JSON
{
    public class ResourceList : List<Resource>, IData
    {
        public ResourceList()
        {
        }

        public ResourceList(List<Resource> list)
        {
            foreach (var el in list)
                Add(el);
        }
    }
}