using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Clojure.Code.Parsing;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureClassifier : ITagger<ClassificationTag>
	{
		private readonly ITextBuffer _buffer;
		private readonly ITagAggregator<ClojureTokenTag> _aggregator;
		private readonly IDictionary<TokenType, IClassificationType> _clojureTypes;
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public ClojureClassifier(ITextBuffer buffer,
		                         ITagAggregator<ClojureTokenTag> clojureTagAggregator,
		                         IClassificationTypeRegistryService typeService)
		{
			_buffer = buffer;
			_aggregator = clojureTagAggregator;
			_aggregator.TagsChanged += TokenTagsChanged;
			_clojureTypes = new Dictionary<TokenType, IClassificationType>();
			_clojureTypes[TokenType.Symbol] = typeService.GetClassificationType("ClojureSymbol");
			_clojureTypes[TokenType.String] = typeService.GetClassificationType("ClojureString");
			_clojureTypes[TokenType.Number] = typeService.GetClassificationType("ClojureNumber");
			_clojureTypes[TokenType.HexNumber] = typeService.GetClassificationType("ClojureNumber");
			_clojureTypes[TokenType.Comment] = typeService.GetClassificationType("ClojureComment");
			_clojureTypes[TokenType.Keyword] = typeService.GetClassificationType("ClojureKeyword");
			_clojureTypes[TokenType.Character] = typeService.GetClassificationType("ClojureCharacter");
			_clojureTypes[TokenType.BuiltIn] = typeService.GetClassificationType("ClojureBuiltIn");
			_clojureTypes[TokenType.Boolean] = typeService.GetClassificationType("ClojureBoolean");
			_clojureTypes[TokenType.ListStart] = typeService.GetClassificationType("ClojureList");
			_clojureTypes[TokenType.ListEnd] = typeService.GetClassificationType("ClojureList");
			_clojureTypes[TokenType.VectorStart] = typeService.GetClassificationType("ClojureVector");
			_clojureTypes[TokenType.VectorEnd] = typeService.GetClassificationType("ClojureVector");
			_clojureTypes[TokenType.MapStart] = typeService.GetClassificationType("ClojureMap");
			_clojureTypes[TokenType.MapEnd] = typeService.GetClassificationType("ClojureMap");
		}

		public void TokenTagsChanged(object sender, TagsChangedEventArgs e)
		{
			foreach (var span in e.Span.GetSpans(_buffer)) TagsChanged(this, new SnapshotSpanEventArgs(span));
		}

		public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			foreach (var tagSpan in _aggregator.GetTags(spans))
			{
				if (!_clojureTypes.ContainsKey(tagSpan.Tag.Token.Type)) continue;
				var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
				yield return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(_clojureTypes[tagSpan.Tag.Token.Type]));
			}
		}
	}

	[Export(typeof (IViewTaggerProvider))]
	[ContentType("Clojure")]
	[TagType(typeof (ClassificationTag))]
	internal sealed class ClojureClassifierProvider : IViewTaggerProvider
	{
		[Export] [Name("Clojure")] [BaseDefinition("code")] internal static ContentTypeDefinition ClojureContentType = null;

		[Export] [FileExtension(".clj")] [ContentType("Clojure")] internal static FileExtensionToContentTypeDefinition ClojureFileType = null;

        [Export] [FileExtension(".cljs")] [ContentType("Clojure")] internal static FileExtensionToContentTypeDefinition ClojureScriptFileType = null;

        
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

		[Import] internal IViewTagAggregatorFactoryService aggregatorFactory = null;

		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
            ITagAggregator<ClojureTokenTag> clojureTagAggregator = aggregatorFactory.CreateTagAggregator<ClojureTokenTag>(textView);
			return new ClojureClassifier(buffer, clojureTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
		}
	}

	internal static class OrdinaryClassificationDefinition
	{
		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureSymbol")] internal static ClassificationTypeDefinition ClojureSymbol = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureString")] internal static ClassificationTypeDefinition ClojureString = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureNumber")] internal static ClassificationTypeDefinition ClojureNumber = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureComment")] internal static ClassificationTypeDefinition ClojureComment = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureKeyword")] internal static ClassificationTypeDefinition ClojureKeyword = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureCharacter")] internal static ClassificationTypeDefinition ClojureCharacter = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureBuiltIn")] internal static ClassificationTypeDefinition ClojureBuiltIn = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureBoolean")] internal static ClassificationTypeDefinition ClojureBoolean = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureList")] internal static ClassificationTypeDefinition ClojureList = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureVector")] internal static ClassificationTypeDefinition ClojureVector = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureMap")] internal static ClassificationTypeDefinition ClojureMap = null;

		[Export(typeof (ClassificationTypeDefinition))] [Name("ClojureMetadataTypeHint")] internal static ClassificationTypeDefinition ClojureMetadataTypeHint = null;
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureSymbol")]
	[Name("ClojureSymbol")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureSymbol : ClassificationFormatDefinition
	{
		public ClojureSymbol()
		{
			DisplayName = "Clojure - Symbol";
			ForegroundColor = Color.FromRgb(0, 0, 128);
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureString")]
	[Name("ClojureString")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureString : ClassificationFormatDefinition
	{
		public ClojureString()
		{
			DisplayName = "Clojure - String";
			ForegroundColor = Color.FromRgb(0, 128, 0);
			IsBold = true;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureNumber")]
	[Name("ClojureNumber")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureNumber : ClassificationFormatDefinition
	{
		public ClojureNumber()
		{
			DisplayName = "Clojure - Number";
			ForegroundColor = Color.FromRgb(0, 0, 255);
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureComment")]
	[Name("ClojureComment")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureComment : ClassificationFormatDefinition
	{
		public ClojureComment()
		{
			DisplayName = "Clojure - Comment";
			ForegroundColor = Color.FromRgb(128, 128, 128);
			IsItalic = true;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureKeyword")]
	[Name("ClojureKeyword")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureKeyword : ClassificationFormatDefinition
	{
		public ClojureKeyword()
		{
			DisplayName = "Clojure - Keyword";
			ForegroundColor = Color.FromRgb(102, 14, 122);
			IsItalic = true;
			IsBold = true;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureCharacter")]
	[Name("ClojureCharacter")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureCharacter : ClassificationFormatDefinition
	{
		public ClojureCharacter()
		{
			DisplayName = "Clojure - Character";
			ForegroundColor = Color.FromRgb(0, 128, 0);
			IsBold = true;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureBuiltIn")]
	[Name("ClojureBuiltIn")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureBuiltIn : ClassificationFormatDefinition
	{
		public ClojureBuiltIn()
		{
			DisplayName = "Clojure - Built In";
			ForegroundColor = Colors.Orange;
			IsBold = true;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureBoolean")]
	[Name("ClojureBoolean")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureBoolean : ClassificationFormatDefinition
	{
		public ClojureBoolean()
		{
			DisplayName = "Clojure - Boolean";
			ForegroundColor = Color.FromRgb(0, 0, 255);
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureList")]
	[Name("ClojureList")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureList : ClassificationFormatDefinition
	{
		public ClojureList()
		{
			DisplayName = "Clojure - List";
			ForegroundColor = Colors.Black;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureVector")]
	[Name("ClojureVector")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureVector : ClassificationFormatDefinition
	{
		public ClojureVector()
		{
			DisplayName = "Clojure - Vector";
			ForegroundColor = Colors.Black;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureMap")]
	[Name("ClojureMap")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureMap : ClassificationFormatDefinition
	{
		public ClojureMap()
		{
			DisplayName = "Clojure - Map";
			ForegroundColor = Colors.Black;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "ClojureMetadataTypeHint")]
	[Name("ClojureMetadataTypeHint")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class ClojureMetadataTypeHint : ClassificationFormatDefinition
	{
		public ClojureMetadataTypeHint()
		{
			DisplayName = "Clojure - Type Hint";
			ForegroundColor = Color.FromRgb(53, 145, 175);
		}
	}
}