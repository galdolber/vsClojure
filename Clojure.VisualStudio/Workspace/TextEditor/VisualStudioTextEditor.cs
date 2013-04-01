using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.TextEditor;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class VisualStudioTextEditor : ITextEditor
	{
		private readonly DTE2 _dte;
		private readonly List<ITextEditorStateChangeListener> _listeners;

        public VisualStudioTextEditor(DTE2 dte, IComponentModel componentModel)
		{
			_dte = dte;
            var editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

            editorFactoryService.TextViewCreated += EditorFactoryServiceOnTextViewCreated;
            _listeners = new List<ITextEditorStateChangeListener>();
		}

	    private void EditorFactoryServiceOnTextViewCreated(object sender, TextViewCreatedEventArgs e)
	    {
            if (!e.TextView.TextSnapshot.ContentType.TypeName.ToLower().StartsWith("clojure"))
            {
                return;
            }

            var vsTextBuffer = e.TextView.TextBuffer;

            if (!vsTextBuffer.Properties.ContainsProperty(typeof(VisualStudioClojureTextBuffer)) || !vsTextBuffer.Properties.ContainsProperty(typeof(ITextDocument)))
            {
                return;
            }

            var clojureTextBuffer = vsTextBuffer.Properties.GetProperty<VisualStudioClojureTextBuffer>(typeof(VisualStudioClojureTextBuffer));
			var filePath = vsTextBuffer.Properties.GetProperty<ITextDocument>(typeof (ITextDocument)).FilePath;

	        new System.Threading.Thread(() =>
	            {
                    //todo: change this to trigger off event handler
                    while (true)
                    {
                        System.Threading.Thread.Sleep(100);

                        try
                        {
                            string activeDocumentFullName = _dte.ActiveDocument.FullName;
                            if (filePath == activeDocumentFullName)
                            {
                                
                            }

                            TextBufferSnapshot textBufferSnapshot = clojureTextBuffer.GetTokenSnapshot();
                            string newSelection = e.TextView.Selection.SelectedSpans.Select(x => x.GetText()).Aggregate((x, y) => x + y);
                            textBufferSnapshot = textBufferSnapshot.ChangeFilePath(filePath);
                            textBufferSnapshot = textBufferSnapshot.ChangeSelection(newSelection);
                            _listeners.ForEach(l => l.OnTextEditorStatusChange(textBufferSnapshot));
                        }
                        catch (Exception)
                        {
                        }
                    }
	            }).Start();
	    }

	    public void AddStateChangeListener(ITextEditorStateChangeListener listener)
		{
			_listeners.Add(listener);
		}

	}
}