using System.Collections.Generic;
using Clojure.Workspace.TextEditor.Options;
using Microsoft.VisualStudio.Text.Editor;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureTextEditorOptions
	{
		private readonly IEditorOptionsFactoryService _optionsFactory;
		private readonly List<IEditorOptionsChangedListener> _listeners;

		public ClojureTextEditorOptions(IEditorOptionsFactoryService optionsFactory)
		{
			_optionsFactory = optionsFactory;
			_listeners = new List<IEditorOptionsChangedListener>();

			_optionsFactory.GlobalOptions.OptionChanged += (o, e) => OptionsChanged();
		}

		private void OptionsChanged()
		{
			var indentSize = _optionsFactory.GlobalOptions.GetOptionValue<int>(new IndentSize().Key);
			var newOptions = new EditorOptions(indentSize);
			_listeners.ForEach(l => l.OnOptionChange(newOptions));
		}

		public void AddOptionsChangedListener(IEditorOptionsChangedListener listener)
		{
			_listeners.Add(listener);

			var indentSize = _optionsFactory.GlobalOptions.GetOptionValue<int>(new IndentSize().Key);
			var newOptions = new EditorOptions(indentSize);

			listener.OnOptionChange(newOptions);
		}
	}
}