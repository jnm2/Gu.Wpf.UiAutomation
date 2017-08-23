﻿namespace Gu.Wpf.UiAutomation.UIA3.EventHandlers
{
    using System;
    using UIA = Interop.UIAutomationClient;

    public class UIA3PropertyChangedEventHandler : PropertyChangedEventHandlerBase, UIA.IUIAutomationPropertyChangedEventHandler
    {
        public UIA3PropertyChangedEventHandler(AutomationBase automation, Action<AutomationElement, PropertyId, object> callAction)
            : base(automation, callAction)
        {
        }

        public void HandlePropertyChangedEvent(UIA.IUIAutomationElement sender, int propertyId, object newValue)
        {
            var basicAutomationElement = new UIA3BasicAutomationElement((UIA3Automation)this.Automation, sender);
            var senderElement = new AutomationElement(basicAutomationElement);
            var property = PropertyId.Find(propertyId);
            this.HandlePropertyChangedEvent(senderElement, property, newValue);
        }
    }
}