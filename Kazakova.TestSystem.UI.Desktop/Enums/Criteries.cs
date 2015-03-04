using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazakova.TestSystem.Logic.Enums
{
    public enum Criteries
    {
        [Description("Критерий покрытия операторов")]
        OperatorsCover,

        [Description("Критерий покрытия решений")]
        SolutionsCover,

        [Description("Критерий покрытия условий")]
        ConditionsCover,

        [Description("Критерий покрытия решений/условий")]
        SolutionsAndConditionsCover,
    }
}
