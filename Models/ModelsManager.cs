using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;

namespace BMHRI.WCS.Server.Models
{
    class ModelsManager
    {
        private static readonly Lazy<ModelsManager> lazy = new Lazy<ModelsManager>(() => new ModelsManager());
        public static ModelsManager Instance { get { return lazy.Value; } }
        public List<BaseModel> ModelsList;
        private ModelsManager()
        {
           ModelsList = MyDataTableExtensions.ToList<BaseModel>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM dbo.SenceModelListBack where ModelType<>0 and ParentID is not null "));
            if (ModelsList == null) ModelsList = new List<BaseModel>();
        }
    }
}
