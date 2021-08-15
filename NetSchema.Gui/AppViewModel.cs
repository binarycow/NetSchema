using NetSchema.Gui.Common;
using NetSchema.Gui.Schema;

namespace NetSchema.Gui
{
    public class AppViewModel : Observable
    {
        
        private SchemaViewModel? selectedSchema;
        public SchemaViewModel? SelectedSchema
        {
            get => this.selectedSchema;
            set => SetField(ref this.selectedSchema, value);
        }
    }
}