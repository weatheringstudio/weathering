

using System;
using System.Collections.Generic;

namespace Weathering
{
    [Depend(typeof(Technology))]
    public class KnowledgeOfHandcraft { }

    public class LibraryOfManufacturing : AbstractTechnologyCenter
    {
        protected override Type TechnologyType => typeof(KnowledgeOfHandcraft);

        protected override List<(Type, long)> TechList => throw new NotImplementedException();
    }
}
