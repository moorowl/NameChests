using System;
using System.Collections.Generic;

namespace NameChests.UserInterface.MenuOptions {
	public class MenuOptionShowOnHover : MenuOptionCycling<ShowOnHoverMode> {
		protected override List<ShowOnHoverMode> AvailableOptions => new() {
			ShowOnHoverMode.None,
			ShowOnHoverMode.Show,
			ShowOnHoverMode.ShowOverrideFacing
		};

		protected override ShowOnHoverMode CurrentOption {
			get => Options.Instance.ShowOnHover;
			set => Options.Instance.ShowOnHover = value;
		}
		
		protected override void UpdateText() {
			valueText.Render(CurrentOption switch {
				ShowOnHoverMode.None => "off",
				ShowOnHoverMode.Show => "on",
				ShowOnHoverMode.ShowOverrideFacing => "NameChests-Options/ShowOnHover_OnOverrideFacing",
				_ => throw new ArgumentOutOfRangeException()
			});
		}
	}
}