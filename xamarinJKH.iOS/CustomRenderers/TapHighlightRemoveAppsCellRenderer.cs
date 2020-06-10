using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH.Apps;
using xamarinJKH.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(AppCell), typeof(TapHighlightRemoveAppsCellRenderer))]
namespace xamarinJKH.iOS.CustomRenderers
{
    public class TapHighlightRemoveAppsCellRenderer : ViewCellRenderer
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