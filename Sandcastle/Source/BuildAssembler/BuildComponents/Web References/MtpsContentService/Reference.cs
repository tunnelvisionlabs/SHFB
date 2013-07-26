﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.17929.
// 
#pragma warning disable 1591

namespace Microsoft.Ddue.Tools.MtpsContentService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ContentServiceBinding", Namespace="urn:msdn-com:public-content-syndication")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(requestedDocument[]))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(primary[]))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(image[]))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(common[]))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(feature[]))]
    public partial class ContentService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private appId appIdValueField;
        
        private System.Threading.SendOrPostCallback GetContentOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetNavigationPathsOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public ContentService() {
            this.Url = global::Microsoft.Ddue.Tools.Properties.Settings.Default.BuildComponents_MtpsContentService_ContentService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public appId appIdValue {
            get {
                return this.appIdValueField;
            }
            set {
                this.appIdValueField = value;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetContentCompletedEventHandler GetContentCompleted;
        
        /// <remarks/>
        public event GetNavigationPathsCompletedEventHandler GetNavigationPathsCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("appIdValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:msdn-com:public-content-syndication/GetContent", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("getContentResponse", Namespace="urn:msdn-com:public-content-syndication")]
        public getContentResponse GetContent([System.Xml.Serialization.XmlElementAttribute(Namespace="urn:msdn-com:public-content-syndication")] getContentRequest getContentRequest) {
            object[] results = this.Invoke("GetContent", new object[] {
                        getContentRequest});
            return ((getContentResponse)(results[0]));
        }
        
        /// <remarks/>
        public void GetContentAsync(getContentRequest getContentRequest) {
            this.GetContentAsync(getContentRequest, null);
        }
        
        /// <remarks/>
        public void GetContentAsync(getContentRequest getContentRequest, object userState) {
            if ((this.GetContentOperationCompleted == null)) {
                this.GetContentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetContentOperationCompleted);
            }
            this.InvokeAsync("GetContent", new object[] {
                        getContentRequest}, this.GetContentOperationCompleted, userState);
        }
        
        private void OnGetContentOperationCompleted(object arg) {
            if ((this.GetContentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetContentCompleted(this, new GetContentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("appIdValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:msdn-com:public-content-syndication/GetNavigationPaths", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("getNavigationPathsResponse", Namespace="urn:msdn-com:public-content-syndication")]
        public getNavigationPathsResponse GetNavigationPaths([System.Xml.Serialization.XmlElementAttribute(Namespace="urn:msdn-com:public-content-syndication")] getNavigationPathsRequest getNavigationPathsRequest) {
            object[] results = this.Invoke("GetNavigationPaths", new object[] {
                        getNavigationPathsRequest});
            return ((getNavigationPathsResponse)(results[0]));
        }
        
        /// <remarks/>
        public void GetNavigationPathsAsync(getNavigationPathsRequest getNavigationPathsRequest) {
            this.GetNavigationPathsAsync(getNavigationPathsRequest, null);
        }
        
        /// <remarks/>
        public void GetNavigationPathsAsync(getNavigationPathsRequest getNavigationPathsRequest, object userState) {
            if ((this.GetNavigationPathsOperationCompleted == null)) {
                this.GetNavigationPathsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetNavigationPathsOperationCompleted);
            }
            this.InvokeAsync("GetNavigationPaths", new object[] {
                        getNavigationPathsRequest}, this.GetNavigationPathsOperationCompleted, userState);
        }
        
        private void OnGetNavigationPathsOperationCompleted(object arg) {
            if ((this.GetNavigationPathsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetNavigationPathsCompleted(this, new GetNavigationPathsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication/2006/09/common")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:msdn-com:public-content-syndication/2006/09/common", IsNullable=false)]
    public partial class appId : System.Web.Services.Protocols.SoapHeader {
        
        private string valueField;
        
        /// <remarks/>
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication")]
    public partial class navigationPathNode {
        
        private string titleField;
        
        private bool isPhantomField;
        
        private bool isPhantomFieldSpecified;
        
        private navigationKey navigationNodeKeyField;
        
        private navigationKey contentNodeKeyField;
        
        /// <remarks/>
        public string title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
        
        /// <remarks/>
        public bool isPhantom {
            get {
                return this.isPhantomField;
            }
            set {
                this.isPhantomField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isPhantomSpecified {
            get {
                return this.isPhantomFieldSpecified;
            }
            set {
                this.isPhantomFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public navigationKey navigationNodeKey {
            get {
                return this.navigationNodeKeyField;
            }
            set {
                this.navigationNodeKeyField = value;
            }
        }
        
        /// <remarks/>
        public navigationKey contentNodeKey {
            get {
                return this.contentNodeKeyField;
            }
            set {
                this.contentNodeKeyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication")]
    public partial class navigationKey {
        
        private string contentIdField;
        
        private string localeField;
        
        private string versionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string contentId {
            get {
                return this.contentIdField;
            }
            set {
                this.contentIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string locale {
            get {
                return this.localeField;
            }
            set {
                this.localeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication")]
    public partial class navigationPath {
        
        private navigationPathNode[] navigationPathNodesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public navigationPathNode[] navigationPathNodes {
            get {
                return this.navigationPathNodesField;
            }
            set {
                this.navigationPathNodesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication")]
    public partial class availableVersionAndLocale {
        
        private string localeField;
        
        private string versionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string locale {
            get {
                return this.localeField;
            }
            set {
                this.localeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication")]
    public partial class requestedDocument {
        
        private string selectorField;
        
        private documentTypes typeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string selector {
            get {
                return this.selectorField;
            }
            set {
                this.selectorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public documentTypes type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:msdn-com:public-content-syndication")]
    public enum documentTypes {
        
        /// <remarks/>
        primary,
        
        /// <remarks/>
        common,
        
        /// <remarks/>
        image,
        
        /// <remarks/>
        feature,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:mtpg-com:mtps/2004/1/primary")]
    public partial class primary {
        
        private System.Xml.XmlElement anyField;
        
        private string primaryFormatField;
        
        private string locationField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="urn:mtpg-com:mtps/2004/1/primary/category")]
        public string primaryFormat {
            get {
                return this.primaryFormatField;
            }
            set {
                this.primaryFormatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location {
            get {
                return this.locationField;
            }
            set {
                this.locationField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:mtpg-com:mtps/2004/1/image")]
    public partial class image {
        
        private string nameField;
        
        private string imageFormatField;
        
        private string locationField;
        
        private byte[] valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="urn:mtpg-com:mtps/2004/1/image/category")]
        public string imageFormat {
            get {
                return this.imageFormatField;
            }
            set {
                this.imageFormatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location {
            get {
                return this.locationField;
            }
            set {
                this.locationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType="base64Binary")]
        public byte[] Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:mtpg-com:mtps/2004/1/common")]
    public partial class common {
        
        private System.Xml.XmlElement[] anyField;
        
        private string commonFormatField;
        
        private string locationField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="urn:mtpg-com:mtps/2004/1/common/category")]
        public string commonFormat {
            get {
                return this.commonFormatField;
            }
            set {
                this.commonFormatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location {
            get {
                return this.locationField;
            }
            set {
                this.locationField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:mtpg-com:mtps/2004/1/feature")]
    public partial class feature {
        
        private System.Xml.XmlElement[] anyField;
        
        private string featureFormatField;
        
        private string locationField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="urn:mtpg-com:mtps/2004/1/feature/category")]
        public string featureFormat {
            get {
                return this.featureFormatField;
            }
            set {
                this.featureFormatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location {
            get {
                return this.locationField;
            }
            set {
                this.locationField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:msdn-com:public-content-syndication")]
    public partial class getContentRequest {
        
        private string contentIdentifierField;
        
        private string localeField;
        
        private string versionField;
        
        private requestedDocument[] requestedDocumentsField;
        
        /// <remarks/>
        public string contentIdentifier {
            get {
                return this.contentIdentifierField;
            }
            set {
                this.contentIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string locale {
            get {
                return this.localeField;
            }
            set {
                this.localeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public requestedDocument[] requestedDocuments {
            get {
                return this.requestedDocumentsField;
            }
            set {
                this.requestedDocumentsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:msdn-com:public-content-syndication")]
    public partial class getContentResponse {
        
        private string contentIdField;
        
        private string contentGuidField;
        
        private string contentAliasField;
        
        private string localeField;
        
        private string versionField;
        
        private availableVersionAndLocale[] availableVersionsAndLocalesField;
        
        private primary[] primaryDocumentsField;
        
        private image[] imageDocumentsField;
        
        private common[] commonDocumentsField;
        
        private feature[] featureDocumentsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string contentId {
            get {
                return this.contentIdField;
            }
            set {
                this.contentIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string contentGuid {
            get {
                return this.contentGuidField;
            }
            set {
                this.contentGuidField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string contentAlias {
            get {
                return this.contentAliasField;
            }
            set {
                this.contentAliasField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string locale {
            get {
                return this.localeField;
            }
            set {
                this.localeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:mtpg-com:mtps/2004/1/key")]
        public string version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public availableVersionAndLocale[] availableVersionsAndLocales {
            get {
                return this.availableVersionsAndLocalesField;
            }
            set {
                this.availableVersionsAndLocalesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("primary", Namespace="urn:mtpg-com:mtps/2004/1/primary", IsNullable=false)]
        public primary[] primaryDocuments {
            get {
                return this.primaryDocumentsField;
            }
            set {
                this.primaryDocumentsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("image", Namespace="urn:mtpg-com:mtps/2004/1/image", IsNullable=false)]
        public image[] imageDocuments {
            get {
                return this.imageDocumentsField;
            }
            set {
                this.imageDocumentsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("common", Namespace="urn:mtpg-com:mtps/2004/1/common", IsNullable=false)]
        public common[] commonDocuments {
            get {
                return this.commonDocumentsField;
            }
            set {
                this.commonDocumentsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("feature", Namespace="urn:mtpg-com:mtps/2004/1/feature", IsNullable=false)]
        public feature[] featureDocuments {
            get {
                return this.featureDocumentsField;
            }
            set {
                this.featureDocumentsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:msdn-com:public-content-syndication")]
    public partial class getNavigationPathsRequest {
        
        private navigationKey rootField;
        
        private navigationKey targetField;
        
        /// <remarks/>
        public navigationKey root {
            get {
                return this.rootField;
            }
            set {
                this.rootField = value;
            }
        }
        
        /// <remarks/>
        public navigationKey target {
            get {
                return this.targetField;
            }
            set {
                this.targetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:msdn-com:public-content-syndication")]
    public partial class getNavigationPathsResponse {
        
        private navigationPath[] navigationPathsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public navigationPath[] navigationPaths {
            get {
                return this.navigationPathsField;
            }
            set {
                this.navigationPathsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetContentCompletedEventHandler(object sender, GetContentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetContentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetContentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public getContentResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((getContentResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetNavigationPathsCompletedEventHandler(object sender, GetNavigationPathsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetNavigationPathsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetNavigationPathsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public getNavigationPathsResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((getNavigationPathsResponse)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591