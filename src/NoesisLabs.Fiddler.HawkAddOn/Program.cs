using Fiddler;
using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thinktecture.IdentityModel.Hawk.Client;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.WebApi;

[assembly: Fiddler.RequiredVersion("2.3.5.0")]
namespace NoesisLabs.Fiddler.HawkAddOn
{
	public class Program : IAutoTamper    // Ensure class is public, or Fiddler won't see it!
	{
		private const string COMPOSER_KEY = "X-From-Builder";
		private const string ICON_KEY = "hawk";
		private const string ICON_RESOURCE_STREAM_NAME = "NoesisLabs.Fiddler.HawkAddOn.hawk.ico";

		private ComboBox algorithmComboBox;
		private CheckBox enableAuthCheckBox;
		private TextBox idTextBox;
		private TextBox keyTextBox;
		private CheckBox limitToComposerCheckBox;

		public string Id { get; set; }
		public string Key { get; set; }

		public Program()
		{
			/* NOTE: It's possible that Fiddler UI isn't fully loaded yet, so don't add any UI in the constructor.

			   But it's also possible that AutoTamper* methods are called before OnLoad (below), so be
			   sure any needed data structures are initialized to safe values here in this constructor */
		}

		public void OnLoad()
		{
			var icon = new Icon(typeof(Program).Assembly.GetManifestResourceStream(ICON_RESOURCE_STREAM_NAME));

			FiddlerApplication.UI.imglSessionIcons.Images.Add(ICON_KEY, icon);

			var page = new TabPage("Hawk Authentication");

			page.ImageIndex = FiddlerApplication.UI.imglSessionIcons.Images.IndexOfKey(ICON_KEY);

			var panel = new TableLayoutPanel() { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 2 };

			var enableAuthLabel = new Label { Text = "Enable:", AutoSize = true, TextAlign = ContentAlignment.BottomLeft };
			panel.Controls.Add(enableAuthLabel);
			panel.SetColumn(enableAuthLabel, 0);
			panel.SetRow(enableAuthLabel, 0);

			this.enableAuthCheckBox = new CheckBox { Checked = false };
			panel.Controls.Add(this.enableAuthCheckBox);
			panel.SetColumn(this.enableAuthCheckBox, 1);
			panel.SetRow(this.enableAuthCheckBox, 0);

			var limitToComposerLabel = new Label { Text = "Composer Only:", AutoSize = true, TextAlign = ContentAlignment.BottomLeft };
			panel.Controls.Add(limitToComposerLabel);
			panel.SetColumn(limitToComposerLabel, 0);
			panel.SetRow(limitToComposerLabel, 1);

			this.limitToComposerCheckBox = new CheckBox { Checked = true };
			panel.Controls.Add(this.limitToComposerCheckBox);
			panel.SetColumn(this.limitToComposerCheckBox, 1);
			panel.SetRow(this.limitToComposerCheckBox, 1);

			var algorithmLabel = new Label { Text = "Algorithm:", AutoSize = true, TextAlign = ContentAlignment.BottomLeft };
			panel.Controls.Add(algorithmLabel);
			panel.SetColumn(algorithmLabel, 0);
			panel.SetRow(algorithmLabel, 2);

			this.algorithmComboBox = new ComboBox();
			foreach (string algorithm in Enum.GetNames(typeof(SupportedAlgorithms)))
			{
				this.algorithmComboBox.Items.Add(algorithm);
			}
			panel.Controls.Add(this.algorithmComboBox);
			panel.SetColumn(this.algorithmComboBox, 1);
			panel.SetRow(this.algorithmComboBox, 2);

			var idLabel = new Label() { Text = "Hawk Id:", AutoSize = true, TextAlign = ContentAlignment.BottomLeft };
			panel.Controls.Add(idLabel);
			panel.SetColumn(idLabel, 0);
			panel.SetRow(idLabel, 3);

			this.idTextBox = new TextBox() { Width = 500 };
			panel.Controls.Add(this.idTextBox);
			panel.SetColumn(this.idTextBox, 1);
			panel.SetRow(this.idTextBox, 3);

			var keyLabel = new Label() { Text = "Hawk Key:", AutoSize = true, TextAlign = ContentAlignment.BottomLeft };
			panel.Controls.Add(keyLabel);
			panel.SetColumn(keyLabel, 0);
			panel.SetRow(keyLabel, 4);

			this.keyTextBox = new TextBox() { Width = 500 };
			panel.Controls.Add(this.keyTextBox);
			panel.SetColumn(this.keyTextBox, 1);
			panel.SetRow(this.keyTextBox, 4);

			page.Controls.Add(panel);

			FiddlerApplication.UI.tabsViews.TabPages.Add(page);
		}

		public void OnBeforeUnload() { }

		public void AutoTamperRequestBefore(Session session) { }

		public void AutoTamperRequestAfter(Session session)
		{
			if (this.enableAuthCheckBox.Checked && (!this.limitToComposerCheckBox.Checked || (this.limitToComposerCheckBox.Checked && session.oFlags.ContainsKey(COMPOSER_KEY))))
			{
				var credential = new Credential() { Id = this.idTextBox.Text, Algorithm = (SupportedAlgorithms)Enum.Parse(typeof(SupportedAlgorithms), (string)this.algorithmComboBox.SelectedItem), User = "", Key = this.keyTextBox.Text };
				
				// GET and POST using the Authorization header
				var options = new ClientOptions() { CredentialsCallback = () => credential }; 

				var client = new HawkClient(options);

				var request = new HttpRequestMessage(new HttpMethod(session.RequestMethod), session.fullUrl);

				foreach(var header in session.oRequest.headers)
				{
					request.Headers.TryAddWithoutValidation(header.Name, header.Value);
				}

				Task.WaitAll(client.CreateClientAuthorizationAsync(new WebApiRequestMessage(request)));

				if (session.oRequest.headers.Exists(HttpRequestHeader.Authorization.ToString()))
				{
					session.oRequest.headers.Remove(HttpRequestHeader.Authorization.ToString());
				}

				session.oRequest.headers.Add(HttpRequestHeader.Authorization.ToString(), request.Headers.Authorization.ToString());
			}
		}
		public void AutoTamperResponseBefore(Session session) { }

		public void AutoTamperResponseAfter(Session session) { }

		public void OnBeforeReturningError(Session session) { }
	}
}