﻿using System;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace SimpleWebTicket
{
    public partial class Default
    {
        /// <summary>
        /// Get webticket for specified user
        /// </summary>
        public void GetWebTicket()
        {
            if (String.IsNullOrEmpty(_userId))
                return;

            if (!String.IsNullOrEmpty(_userGroups))
                GetGroups();

            string webTicketXml = string.Format("<Global method=\"GetWebTicket\"><UserId>{0}</UserId>{1}</Global>", _userId, _userGroups);

            string result = Execute(String.Format("{0}QvAJAXZfc/GetWebTicket.aspx", AccessPointServer), "POST", webTicketXml);

            if (string.IsNullOrEmpty(result) || result.Contains("Invalid call"))
                return;

            XDocument doc = XDocument.Parse(result);

            _webTicket = doc.Root.Element("_retval_").Value;

            // Set friendly name cookie for AccessPoint
            if (!String.IsNullOrEmpty(_userFriendlyName))
            {
                var cookie = new HttpCookie("WelcomeName" + HttpUtility.UrlEncode(_userId)) { Value = _userFriendlyName, Path = String.Format("{0}QvAJAXZfc/", AccessPointServer) };
                Response.Cookies.Add(cookie);
            }
        }

        public void GetGroups()
        {
            var group = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(_userGroups))
            {
                group.Append("<GroupList>");

                foreach (string value in _userGroups.Split(';'))
                {
                    group.Append("<string>");
                    group.Append(value);
                    group.Append("</string>");
                }

                group.Append("</GroupList>");
                group.Append("<GroupsIsNames>");
                group.Append("true");
                group.Append("</GroupsIsNames>");
            }
            
            _userGroups = group.ToString();
        }

        /// <summary>
        /// Redirects to QlikView after succesfull retrieval of webticket
        /// </summary>
        public void RedirectToQlikView()
        {
            Response.Redirect(string.Format("{0}QvAJAXZfc/Authenticate.aspx?type=html&webticket={1}&try={2}&back={3}", AccessPointServer, _webTicket, TryUrl, BackUrl));
        }
    }
}