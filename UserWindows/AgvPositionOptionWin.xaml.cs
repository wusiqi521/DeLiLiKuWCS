using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// AgvPositionOptionWin.xaml 的交互逻辑
    /// </summary>
    public partial class AgvPositionOptionWin : Window
    {
        public AgvPositionOptionWin()
        {
            InitializeComponent();
        }

        public AgvPositionOptionWin(AgvPosition agvPosition)
        {
            InitializeComponent();
            DataContext = agvPosition;
        }

        private void SaveBTClick(object sender, RoutedEventArgs e)
        {
            AgvPosition agvPosition = DataContext as AgvPosition;
            if (agvPosition == null) return;

            SqlParameter[] sqlParameters = new SqlParameter[]
           {
               new SqlParameter("@Position",(agvPosition.Position)),
               new SqlParameter("@PositionStatus",SqlNull(agvPosition.PositionStatus)),
               new SqlParameter("@PalletNo",SqlNull(agvPosition.PalletNo)),
               new SqlParameter("@IsAvailable",SqlNull(agvPosition.IsAvailable)),
               new SqlParameter("@PositionType",SqlNull(agvPosition.PositionType)),
               new SqlParameter("@FLLPosition",SqlNull(agvPosition.FLLPosition)),
               new SqlParameter("@FLPosition",SqlNull(agvPosition.FLPosition)),
               new SqlParameter("@Describe",SqlNull(agvPosition.Describe)),
           };
            SQLServerHelper.ExeSQLStringWithParam("UPDATE[dbo].[AgvPositionList] SET " +
                "  [IsAvailable] = @IsAvailable" +
                ", [PalletNo] = @PalletNo" +
                ", [FLPosition] = @FLPosition" +
                ", [FLLPosition] = @FLLPosition" +
                ", [PositionType] = @PositionType" +
                ", [PositionStatus] = @PositionStatus" +
                ", [Describe] = @Describe" +
                " WHERE [Position] = @Position", sqlParameters);
        }
        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }
    }
}
