namespace Sitecore.Support.XA.Feature.SiteMetadata.Repositories.BrowserTitle
{
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore.Data.Items;
  using Sitecore.DependencyInjection;
  using Sitecore.XA.Feature.SiteMetadata.Models;
  using Sitecore.XA.Feature.SiteMetadata.Repositories.BrowserTitle;
  using Sitecore.XA.Foundation.Abstractions;
  using Sitecore.XA.Foundation.Multisite;
  using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
  using Sitecore.XA.Foundation.SitecoreExtensions.Interfaces;
  using Sitecore.XA.Foundation.Variants.Abstractions.Models;
  using Sitecore.XA.Foundation.Variants.Abstractions.Renderers;
  using System;
  using System.Text.RegularExpressions;
  using Templates = Sitecore.XA.Feature.SiteMetadata.Templates;

  public class BrowserTitleRepository : IBrowserTitleRepository
  {
    protected IContext Context { get; } = ServiceLocator.ServiceProvider.GetService<IContext>();

    protected virtual Item GetSettingsItem(Item item)
    {
      Item settingsItem = ServiceLocator.ServiceProvider.GetService<IMultisiteContext>().GetSettingsItem(item);
      return settingsItem.FirstChildInheritingFrom(Templates.BrowserTitleDefinition.ID);
    }

    protected virtual string GetDefaultValue()
    {
      IRendering rendering = ServiceLocator.ServiceProvider.GetService<IRendering>();
      var displayName = rendering.Item.DisplayName;
      if (String.IsNullOrEmpty(displayName))
      {
        displayName = rendering.Item.Name;
      }
      return displayName;
    }

    public BrowserTitleRenderingModel GetModel()
    {
      return GetModel(Context.Item);
    }

    public BrowserTitleRenderingModel GetModel(Item item)
    {
      var settingsItem = GetSettingsItem(item);
      if (settingsItem != null)
      {
        var renderer = ServiceLocator.ServiceProvider.GetService<IVariantRenderer>();
        renderer.SetParams(false, RendererMode.Html);
        #region Changed code
        return new BrowserTitleRenderingModel { Title = Regex.Replace(renderer.RenderVariant(settingsItem, item), "<[^>]*(>|$)", string.Empty) }; 
        #endregion
      }

      return new BrowserTitleRenderingModel { Title = GetDefaultValue() };
    }
  }
}