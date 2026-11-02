
using System.ComponentModel;
using System.Windows.Controls;
using RESULT_ANALYTICS.ViewModels;

namespace RESULT_ANALYTICS.Views
{
    public partial class ThongKeCanhBaoView : UserControl
    {
        public ThongKeCanhBaoView()
        {
            InitializeComponent();

            // Khoi tao ViewModel khi chay ung dung
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new ThongKeCanhBaoViewModel();
            }
        }
    }
}
