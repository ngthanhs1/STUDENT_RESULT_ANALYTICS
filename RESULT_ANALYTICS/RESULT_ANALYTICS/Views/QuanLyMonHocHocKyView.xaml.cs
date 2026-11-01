using RESULT_ANALYTICS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RESULT_ANALYTICS.Views
{
    public partial class QuanLyMonHocHocKyView : UserControl
    {
        // Ham khoi tao giao dien
        public QuanLyMonHocHocKyView()
        {
            InitializeComponent();

            // Khoi tao ViewModel khi chay ung dung
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new QuanLyMonHocHocKyViewModel();
            }
        }
    }
}
