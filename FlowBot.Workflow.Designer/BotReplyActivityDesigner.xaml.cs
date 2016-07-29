using FlowBotActivityLibrary;
using System;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlowBotActivityLibrary
{
    // Interaction logic for BotReplyActivityDesigner.xaml
    public partial class BotReplyActivityDesigner
    {
        public BotReplyActivityDesigner()
        {
            InitializeComponent();
        }
        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(BotReplyActivity),
                new DesignerAttribute(typeof(BotReplyActivityDesigner)),
                new DescriptionAttribute("Bot Reply Activity"),
                new ToolboxBitmapAttribute(typeof(BotReplyActivity), "BotReplyActivity.png"));
        }

    }
}
