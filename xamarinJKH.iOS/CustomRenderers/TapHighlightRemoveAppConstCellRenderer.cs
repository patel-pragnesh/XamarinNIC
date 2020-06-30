using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH.AppsConst;
using xamarinJKH.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(AppConstCell), typeof(TapHighlightRemoveAppConstCellRenderer))]
namespace xamarinJKH.iOS.CustomRenderers
{
    public class TapHighlightRemoveAppConstCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            // убираем стиль по-умлочанию - серый цвет вокруг поля ячейки при нажатии
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

    }
}