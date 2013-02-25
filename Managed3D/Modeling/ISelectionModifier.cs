﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managed3D.Modeling
{
    public interface ISelectionModifier
    {
        #region Properties
        SelectionAction Action
        {
            get;
            set;
        }

        SelectionTarget Target
        {
            get;
            set;
        }
        #endregion
        #region Methods

        #endregion
    }
}