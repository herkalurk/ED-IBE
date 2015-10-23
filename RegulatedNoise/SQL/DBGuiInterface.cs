﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RegulatedNoise.Enums_and_Utility_Classes;

namespace RegulatedNoise.SQL
{
    class DBGuiInterface
    {
        String m_InitGroup;
        Object m_currentLoadingObject = null;

#region  TagParts

        private class TagParts
        {
            public String IDString { get; set; }
            public String DefaultValue { get; set; }
        }


#endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="InitGroup"></param>
        public DBGuiInterface(String InitGroup)
        {
            try
            {
                m_InitGroup = InitGroup;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating object", ex);
            }
        }

        /// <summary>
        /// saves a setting to the database
        /// </summary>
        /// <param name="sender"></param>
        public Boolean saveSetting(object sender, Object Param1 = null)
        {
            Boolean retValue = false;

            try
            {
                if(m_currentLoadingObject != sender)
                { 
                    if(sender.GetType() == typeof(CheckBox))
                    {
                        var cbSender = (CheckBox)sender;
                        var Parts    = splitTag(cbSender.Tag);    

                        if(Parts != null)
                            retValue = Program.DBCon.setIniValue(m_InitGroup, Parts.IDString, cbSender.Checked.ToString());
                    }
                    else if((sender.GetType() == typeof(ComboBox)) || (sender.GetType() == typeof(ComboBoxInt32)))
                    {
                        var cbSender = (ComboBox)sender;
                        var Parts    = splitTag(cbSender.Tag);    

                        if(Parts != null)
                            retValue = Program.DBCon.setIniValue(m_InitGroup, Parts.IDString, cbSender.Text);
                    }
                    else if((sender.GetType() == typeof(TextBox)) || (sender.GetType() == typeof(TextBoxInt32)))
                    {
                        var cbSender = (TextBox)sender;
                        var Parts    = splitTag(cbSender.Tag);    

                        if(Parts != null)
                            retValue = Program.DBCon.setIniValue(m_InitGroup, Parts.IDString, cbSender.Text);
                    }
                    else if(sender.GetType() == typeof(RadioButton))
                    {
                        // radio button will be set due to its parent container
                        var cbSender = (RadioButton)sender;

                        // avoid double saving (one is activated, another is deactivated)
                        if(cbSender.Checked)
                            retValue = saveSetting(cbSender.Parent);
                    }

                    else if(sender.GetType() == typeof(GroupBox))
                    {
                        // a container will handle it's radiobuttons
                        var cbSender = (GroupBox)sender;
                        var Parts    = splitTag(cbSender.Tag);    
                    
                        if(Parts != null)
                        {
                            Boolean Found = false;

                            // Search all radionbuttons in the combobox and 
                            // check this one with the <Value> tag. Or if not found
                            // set the fist found RadioButton checked

                            foreach (Control SubControl in cbSender.Controls)
                            {
                                if(SubControl.GetType() == typeof(RadioButton))
                                {
                                    RadioButton rbControl = (RadioButton)SubControl;

                                    if(rbControl.Checked)
                                    {
                                        retValue = Program.DBCon.setIniValue(m_InitGroup, Parts.IDString, rbControl.Tag.ToString());
                                        Found = true;
                                        break;
                                    }
                                }
                            }

                            if(!Found)
                                retValue = Program.DBCon.setIniValue(m_InitGroup, Parts.IDString, Parts.DefaultValue);

                        }
                    }
                    else if(sender.GetType() == typeof(DataGridViewExt))
                    {
                        var cbSender = (DataGridViewExt)sender;
                        var Parts    = splitTag(cbSender.Tag);    

                        if(Param1 != null)
                        {
                            if(Param1.GetType() == typeof(DataGridViewExt.SortedEventArgs))
                            {
                                var SortEA = (DataGridViewExt.SortedEventArgs)Param1;

                                // sortorder changed
                                retValue  = Program.DBCon.setIniValue(m_InitGroup, Parts.IDString + "_SortColumn", SortEA.SortColumn.Name);
                                retValue |= Program.DBCon.setIniValue(m_InitGroup, Parts.IDString + "_SortOrder", SortEA.SortOrder.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cErr.showError(ex, "Error in saveSetting");
            }

            return retValue;
        }

        /// <summary>
        ///loads a setting from the database
        /// </summary>
        /// <param name="sender"></param>
        public void loadSetting(object sender, Control BaseObject = null)
        {
            try
            {
                /// set the value of the baseobject if necessary
                if(sender.GetType() == typeof(CheckBox))
                {
                    var cbSender = (CheckBox)sender;
                    var Parts    = splitTag(cbSender.Tag);    

                    if(Parts != null)
                    {
                        m_currentLoadingObject = cbSender;
                        cbSender.Checked       = Program.DBCon.getIniValue<Boolean>(m_InitGroup, Parts.IDString, Parts.DefaultValue, false, true);
                        m_currentLoadingObject = null;
                    }
                }
                else if((sender.GetType() == typeof(ComboBox)) || (sender.GetType() == typeof(ComboBoxInt32)))
                {
                    var cbSender = (ComboBox)sender;
                    var Parts    = splitTag(cbSender.Tag);    

                    if(Parts != null)
                    {
                        m_currentLoadingObject = cbSender;
                        cbSender.Text          = Program.DBCon.getIniValue<String>(m_InitGroup, Parts.IDString, Parts.DefaultValue, false, true);
                        m_currentLoadingObject = null;
                    }
                }
                else if((sender.GetType() == typeof(TextBox)) || (sender.GetType() == typeof(TextBoxInt32)))
                {
                    var cbSender = (TextBox)sender;
                    var Parts    = splitTag(cbSender.Tag);    

                    if(Parts != null)
                    {
                        m_currentLoadingObject = cbSender;
                        cbSender.Text          = Program.DBCon.getIniValue<String>(m_InitGroup, Parts.IDString, Parts.DefaultValue, false, true);
                        m_currentLoadingObject = null;
                    }
                }
                else if(sender.GetType() == typeof(RadioButton))
                {
                    // radio button will be set due to its parent container
                    var cbSender = (RadioButton)sender;

                    if(cbSender.Parent != BaseObject)
                        // avoid recursion
                        loadSetting(cbSender.Parent);
                }

                else if(sender.GetType() == typeof(GroupBox))
                {
                    // a container will handle it's radiobuttons
                    var cbSender = (GroupBox)sender;
                    var Parts    = splitTag(cbSender.Tag);    
                    
                    if(Parts != null)
                    {
                        RadioButton firstRadioButton = null;
                        Boolean Found = false;
                        String Value;

                        m_currentLoadingObject  = cbSender;
                        Value                   = Program.DBCon.getIniValue<String>(m_InitGroup, Parts.IDString, Parts.DefaultValue, false, true);
                        m_currentLoadingObject  = null;

                        // Search all radionbuttons in the combobox and 
                        // check this one with the <Value> tag. Or if not found
                        // set the fist found RadioButton checked

                        foreach (Control SubControl in cbSender.Controls)
                        {
                            if(SubControl.GetType() == typeof(RadioButton))
                            {
                                RadioButton rbControl = (RadioButton)SubControl;

                                if(firstRadioButton == null)
                                    firstRadioButton = rbControl;
                                
                                if(rbControl.Tag.Equals(Value))
                                {
                                    rbControl.Checked = true;
                                    Found = true;
                                }
                                else
                                {
                                    rbControl.Checked = false;
                                }

                            }
                        }

                        if(!Found)
                            firstRadioButton.Checked = true;

                    }
                }
                else if(sender.GetType() == typeof(DataGridViewExt))
                {
                    var cbSender = (DataGridViewExt)sender;
                    var Parts    = splitTag(cbSender.Tag);    
                    System.ComponentModel.ListSortDirection Order;
                    String OrderStr = "";

                    // sortorder changed

                    if(Parts != null)
                    {
                        var Column    = cbSender.Columns[Program.DBCon.getIniValue<String>(m_InitGroup, Parts.IDString + "_SortColumn", Parts.DefaultValue, false, true)];
                        OrderStr      = Program.DBCon.getIniValue<String>(m_InitGroup, Parts.IDString + "_SortOrder", SortOrder.Ascending.ToString(), false, true);

                        if(OrderStr.Equals(SortOrder.Descending.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            Order = System.ComponentModel.ListSortDirection.Descending;
                        else 
                            Order = System.ComponentModel.ListSortDirection.Ascending;
                        
                        if(Column != null)
                        {
                            m_currentLoadingObject = cbSender;
                            cbSender.Sort(Column,Order);
                            m_currentLoadingObject = null;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                m_currentLoadingObject = null;
                cErr.showError(ex, "Error in loadSetting");
            }
        }

        /// <summary>
        /// loads recursive all needed values 
        /// </summary>
        /// <param name="BaseObject"></param>
        internal void loadAllSettings(Control BaseObject, Control BaseBaseObject = null)
        {
            try
            {
                loadSetting(BaseObject, BaseBaseObject);

                // check if there are sub-objects who need values
                foreach (Control SubObject in BaseObject.Controls)
                    loadAllSettings(SubObject, BaseObject);

            }
            catch (Exception ex)
            {
                throw new Exception("Error while loading all setting from DB", ex);
            }
        }

        /// <summary>
        /// splits the parts of a tag-string
        /// </summary>
        /// <param name="TagString"></param>
        /// <returns></returns>
        private TagParts splitTag(Object TagString)
        {
            TagParts TParts     = null;

            try
            {
                if(TagString != null)
                { 
                    String[] Parts      = ((String)TagString).Split(';');

                    if(Parts.GetUpperBound(0) == 1)
                    { 
                        TParts              = new TagParts();

                        TParts.IDString     = Parts[0];
                        TParts.DefaultValue = Parts[1];
                    }
                }

                return TParts;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while splitting tag", ex);
            }
        }
    }
}