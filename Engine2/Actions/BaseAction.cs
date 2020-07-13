using System;
using Engine.Models;

namespace Engine.Actions
{
    public abstract class BaseAction
    {
        protected GameItem _itemInUse;

        protected BaseAction(GameItem itemInUse) => _itemInUse = itemInUse;

        public event EventHandler<string> OnActionPerformed;

        protected void ReportResult(string result)
        {
            OnActionPerformed?.Invoke(this, result);
        }
    }
}