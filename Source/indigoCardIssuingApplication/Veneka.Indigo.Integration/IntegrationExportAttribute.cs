using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Veneka.Indigo.Integration
{
    /// <summary>
    /// Class used to provide a type safe attribute for MEF Exports and Imports. 
    /// The properties will be used to distinguish the same Imported parts for use by different Issuers.
    /// 
    /// Example: IssuerA might require the use of a different core banking system than IssuerB, 
    ///             Indigo will allow the issuers to be setup using the specified core banking interfaces.    
    /// </summary>
    ///  <remarks>
    ///  Inherits <see cref="System.ComponentModel.Composition.ExportAttribute"/>
    ///  </remarks> 
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IntegrationExportAttribute : ExportAttribute
    {
        public string IntegrationName { get; set; }
        public string IntegrationGUID { get; set; }

        /// <summary>
        /// Specifies the unique name and GUID for an Export.
        /// </summary>
        /// <param name="name">
        /// This is the name of the interface, it may not be an empty string or <see langword="null"/>. 
        /// </param>
        /// <param name="guid">
        /// This is the is the <see cref="T:System.Guid"/> of the interface and should be unique for all interfaces.
        /// </param>
        /// <param name="contractType">
        /// This is the <see cref="System.Type"/>
        /// </param>
        public IntegrationExportAttribute(string name, string guid, Type contractType)
            : base(contractType)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Attribute with name 'argument' cannot be null or empty");

            if (String.IsNullOrWhiteSpace(guid))
                throw new ArgumentNullException("Attribute with name 'guid' argument cannot be null or empty");

            Guid guidOut;
            if(!Guid.TryParse(guid, out guidOut))
                throw new ArgumentException("Value of attribute with name 'guid' is not a valid GUID.");
            
            this.IntegrationName = name;
            this.IntegrationGUID = guid;
        }
    }    

    /// <summary>
    /// An intreface providing the read only properties for the MetaData for MEF to hook up to.
    /// </summary>
    public interface IIntegrationCapabilities
    {
        string IntegrationName { get; }
        string IntegrationGUID { get; }
    }
}
