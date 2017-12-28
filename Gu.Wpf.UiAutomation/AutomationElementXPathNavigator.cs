﻿namespace Gu.Wpf.UiAutomation
{
    using System;
    using System.Windows.Automation;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Custom implementation of a <see cref="XPathNavigator" /> which allows
    /// selecting items by xpath by using the <see cref="TreeWalker" />.
    /// </summary>
    public class AutomationElementXPathNavigator : XPathNavigator
    {
        private const int NoAttributeValue = -1;
        private readonly UiElement rootElement;
        private readonly TreeWalker treeWalker;
        private AutomationElement currentElement;
        private int attributeIndex = NoAttributeValue;

        public AutomationElementXPathNavigator(UiElement rootElement)
        {
            this.treeWalker = TreeWalker.ControlViewWalker;
            this.rootElement = rootElement;
            this.currentElement = rootElement.AutomationElement;
        }

        private enum ElementAttributes
        {
            AutomationId,
            Name,
            ClassName,
            HelpText
        }

        /// <inheritdoc/>
        public override bool HasAttributes => !this.IsInAttribute;

        /// <inheritdoc/>
        public override string Value => this.IsInAttribute ? this.GetAttributeValue(this.attributeIndex) : this.currentElement.ToString();

        /// <inheritdoc/>
        public override object UnderlyingObject => this.currentElement;

        /// <inheritdoc/>
        public override XPathNodeType NodeType
        {
            get
            {
                if (this.IsInAttribute)
                {
                    return XPathNodeType.Attribute;
                }

                if (this.currentElement.Equals(this.rootElement))
                {
                    return XPathNodeType.Root;
                }

                return XPathNodeType.Element;
            }
        }

        /// <inheritdoc/>
        public override string LocalName => this.IsInAttribute ? this.GetAttributeName(this.attributeIndex) : this.currentElement.Properties.ControlType.Value.ToString();

        /// <inheritdoc/>
        public override string Name => this.LocalName;

        /// <inheritdoc/>
        public override XmlNameTable NameTable => throw new NotImplementedException();

        /// <inheritdoc/>
        public override string NamespaceURI => string.Empty;

        /// <inheritdoc/>
        public override string Prefix => string.Empty;

        /// <inheritdoc/>
        public override string BaseURI => string.Empty;

        /// <inheritdoc/>
        public override bool IsEmptyElement => false;

        private bool IsInAttribute => this.attributeIndex != NoAttributeValue;

        /// <inheritdoc/>
        public override XPathNavigator Clone()
        {
            var clonedObject = new AutomationElementXPathNavigator(this.rootElement)
            {
                currentElement = this.currentElement,
                attributeIndex = this.attributeIndex
            };
            return clonedObject;
        }

        /// <inheritdoc/>
        public override bool MoveToFirstAttribute()
        {
            if (this.IsInAttribute)
            {
                return false;
            }

            this.attributeIndex = 0;
            return true;
        }

        /// <inheritdoc/>
        public override bool MoveToNextAttribute()
        {
            if (this.attributeIndex >= Enum.GetNames(typeof(ElementAttributes)).Length - 1)
            {
                // No more attributes
                return false;
            }

            if (!this.IsInAttribute)
            {
                return false;
            }

            this.attributeIndex++;
            return true;
        }

        /// <inheritdoc/>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            if (this.IsInAttribute)
            {
                return string.Empty;
            }

            var attributeIndex = this.GetAttributeIndexFromName(localName);
            if (attributeIndex != NoAttributeValue)
            {
                return this.GetAttributeValue(attributeIndex);
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            if (this.IsInAttribute)
            {
                return false;
            }

            var attributeIndex = this.GetAttributeIndexFromName(localName);
            if (attributeIndex != NoAttributeValue)
            {
                this.attributeIndex = attributeIndex;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void MoveToRoot()
        {
            this.attributeIndex = NoAttributeValue;
            this.currentElement = this.rootElement;
        }

        /// <inheritdoc/>
        public override bool MoveToNext()
        {
            if (this.IsInAttribute)
            {
                return false;
            }

            var nextElement = this.treeWalker.GetNextSibling(this.currentElement);
            if (nextElement == null)
            {
                return false;
            }

            this.currentElement = nextElement;
            return true;
        }

        /// <inheritdoc/>
        public override bool MoveToPrevious()
        {
            if (this.IsInAttribute)
            {
                return false;
            }

            var previousElement = this.treeWalker.GetPreviousSibling(this.currentElement);
            if (previousElement == null)
            {
                return false;
            }

            this.currentElement = previousElement;
            return true;
        }

        /// <inheritdoc/>
        public override bool MoveToFirstChild()
        {
            if (this.IsInAttribute)
            {
                return false;
            }

            var childElement = this.treeWalker.GetFirstChild(this.currentElement);
            if (childElement == null)
            {
                return false;
            }

            this.currentElement = childElement;
            return true;
        }

        /// <inheritdoc/>
        public override bool MoveToParent()
        {
            if (this.IsInAttribute)
            {
                this.attributeIndex = NoAttributeValue;
                return true;
            }

            if (this.currentElement.Equals(this.rootElement))
            {
                return false;
            }

            this.currentElement = this.treeWalker.GetParent(this.currentElement);
            return true;
        }

        /// <inheritdoc/>
        public override bool MoveTo(XPathNavigator other)
        {
            var specificNavigator = other as AutomationElementXPathNavigator;
            if (specificNavigator == null)
            {
                return false;
            }

            if (!this.rootElement.Equals(specificNavigator.rootElement))
            {
                return false;
            }

            this.currentElement = specificNavigator.currentElement;
            this.attributeIndex = specificNavigator.attributeIndex;
            return true;
        }

        /// <inheritdoc/>
        public override bool MoveToId(string id)
        {
            return false;
        }

        /// <inheritdoc/>
        public override bool IsSamePosition(XPathNavigator other)
        {
            var specificNavigator = other as AutomationElementXPathNavigator;
            if (specificNavigator == null)
            {
                return false;
            }

            if (!this.rootElement.Equals(specificNavigator.rootElement))
            {
                return false;
            }

            return this.currentElement.Equals(specificNavigator.currentElement)
                && this.attributeIndex == specificNavigator.attributeIndex;
        }

        private string GetAttributeValue(int attributeIndex)
        {
            switch ((ElementAttributes)attributeIndex)
            {
                case ElementAttributes.AutomationId:
                    return this.currentElement.AutomationId();
                case ElementAttributes.Name:
                    return this.currentElement.Name();
                case ElementAttributes.ClassName:
                    return this.currentElement.ClassName();
                case ElementAttributes.HelpText:
                    return this.currentElement.HelpText();
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeIndex));
            }
        }

        private string GetAttributeName(int attributeIndex)
        {
            var name = Enum.GetName(typeof(ElementAttributes), attributeIndex);
            if (name == null)
            {
                throw new ArgumentOutOfRangeException(nameof(attributeIndex));
            }

            return name;
        }

        private int GetAttributeIndexFromName(string attributeName)
        {
            if (Enum.TryParse(attributeName, out ElementAttributes parsedValue))
            {
                return (int)parsedValue;
            }

            return NoAttributeValue;
        }
    }
}
