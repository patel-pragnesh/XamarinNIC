
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using xamarinJKH.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(ViewCell), typeof(TapHighlightRemoveViewCellRenderer))]

namespace xamarinJKH.iOS.CustomRenderers
{
   public  class TapHighlightRemoveViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);

            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = Color.Transparent.ToUIColor(),
            };
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            // убираем стиль по-умлочанию - серый цвет вокруг поля ячейки при нажатии
            //cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }
}