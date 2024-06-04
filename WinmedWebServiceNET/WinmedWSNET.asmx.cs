using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Data;
using Sybase.Data.AseClient;
using System.Security.Cryptography;

namespace WinmedWebServiceNET
{
    /// <summary>
    /// Descripción breve de WinmedWSNET
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WinmedWSNET : WebService
    {
        /*[WebMethod]
        public string HelloWorld()
        {
            //Data Source=xxx.xxx.xxx.xxx; Port=1234; Database=dbname; Uid=username; Pwd=password; ConnectionIdleTimeout=nnn;Charset=iso_1;
            //Host=150.130.102.56;Port=4032;User ID=conswmed;Password=vitamed1;Database Name=audiorespuesta_prd;

            //*************** hasta aquí va con madre
            //nomrbeSP: sp_aud_busca_nombre_prov
            //nombreParam: @ps_clave_proveedor

            AseConnection sysConn = new AseConnection();
                sysConn.ConnectionString = "Data Source=150.130.102.56; Port=4032; Database=audiorespuesta_prd; Uid=conswmed; Pwd=vitamed1; ConnectionIdleTimeout=400; Charset=iso_1;";
                sysConn.Open();

                string mNombre = "";
                //*************** hasta aquí va con madre
                //nomrbeSP: sp_aud_busca_nombre_prov
                //nombreParam: @ps_clave_proveedor

                AseCommand cmd = sysConn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.sp_aud_busca_nombre_prov";

                AseParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = "ps_clave_proveedor";
                parameter.Value = "00003433";
                cmd.Parameters.Add(parameter);
                
                AseDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    mNombre = reader.GetString(0);
                    //string id = reader.GetString(1);
                    //decimal price = reader.GetDecimal(2);
                }
                reader.Close();
                sysConn.Close();
            
            return "Hola a todos";
        }*/

        private string getFormatoFecha(string Fecha, bool FechaSimple)
        {
            DateTime dateAux = new DateTime();
            string retFecha = Fecha;
            int Hora;

            if (DateTime.TryParse(Fecha, out dateAux))
            {
                if (FechaSimple)
                    retFecha = dateAux.ToString("yyyy-MM-dd");
                else
                    retFecha = dateAux.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                if (Fecha.Length > 10)
                {
                    if (FechaSimple)
                    {
                        retFecha = Fecha.Substring(0, 10);
                        string[] valores = retFecha.Split('/');
                        retFecha = valores[2] + "-" + valores[1] + "-" + valores[0];
                    }
                    else
                    {
                        //      07/02/1984 03:00:00 p. m.
                        string dia = Fecha.Substring(0, 2);
                        string mes = Fecha.Substring(3, 2);
                        string año = Fecha.Substring(6, 4);
                        string hora = Fecha.Substring(11, 2);
                        string min = Fecha.Substring(14, 2);
                        string sec = Fecha.Substring(17, 2);
                        string meridiano = Fecha.Substring(20, 1);

                        if (hora == "12")
                            hora = "00";

                        if (meridiano == "p")
                        {
                            //int Hora = Convert.ToInt32(hora);
                            if(int.TryParse(hora, out Hora))
                                hora = (Hora + 12).ToString();
                        }

                        retFecha = año + "-" + mes + "-" + dia + " " + hora + ":" + min + ":" + sec;
                    }
                }
            }

            return retFecha;
        }

        #region Medico
        
        //***************************************************************** Medico
        [WebMethod] // #01
        public string getMedicoNombre(string MedicoID)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_clave_proveedor", MedicoID));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_busca_nombre_prov", lstParam);

                string pNombre = dtResult.Rows[0][0].ToString();
                string pApePat = dtResult.Rows[0][1].ToString();
                string pApeMat = dtResult.Rows[0][2].ToString();
                string pReclamos = dtResult.Rows[0][3].ToString();
                string pReclamoEstatus = dtResult.Rows[0][4].ToString();

                retValue = "<proot>" +
                    "<error>00</error><desc></desc>" +
                    "<name>"+ pNombre + "</name>" +
                    "<pat>" + pApePat + "</pat>" +
                    "<mat>" + pApeMat + "</mat>" +
                    "<reclamCant>" + pReclamos + "</reclamCant>" +
                    "<reclamEst>" + pReclamoEstatus + "</reclamEst>" +
                "</proot>";
            }
            catch (AseException sybex)
            {
                retValue = "<proot><error>-03</error><desc>" + sybex.ToString() + "</desc></proot>";
            }
            catch (Exception ex)
            {
                retValue = "<proot><error>-02</error><desc>" + ex.ToString() + "</desc></proot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #02
        public string getMedicoDomicilio(string MedicoID)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_clave_proveedor", MedicoID));

                dtResult = sysConn.sybaseGetTable("dbo.sps_aud_portal_busca_dom_prov", lstParam);

                string pCalle = dtResult.Rows[0][0].ToString();
                string pNumExt = dtResult.Rows[0][1].ToString();
                string pNumInt = dtResult.Rows[0][2].ToString();
                string pColonia = dtResult.Rows[0][3].ToString();
                string pEstado = dtResult.Rows[0][4].ToString();
                string pCP = dtResult.Rows[0][5].ToString();
                string pCiudad = dtResult.Rows[0][6].ToString();
                string pCedula = dtResult.Rows[0][7].ToString();
                string pCedulaFecha = dtResult.Rows[0][8].ToString();

                retValue = "<proot>" +
                    "<error>00</error><desc></desc>" +
                    "<calle>" + pCalle + "</calle>" +
                    "<no_ext>" + pNumExt + "</no_ext>" +
                    "<no_int>" + pNumInt + "</no_int>" +
                    "<colonia>" + pColonia + "</colonia>" +
                    "<estado>" + pEstado + "</estado>" +
                    "<cp>" + pCP + "</cp>" +
                    "<ciudad>" + pCiudad + "</ciudad>" +
                    "<cedula>" + pCedula + "</cedula>" +
                    "<fecha_cedula>" + getFormatoFecha(pCedulaFecha, false) + "</fecha_cedula>" +
                "</proot>";
            }
            catch (AseException sybex)
            {
                retValue = "<proot><error>-03</error><desc>" + sybex.ToString() + "</desc></proot>";
            }
            catch (Exception ex)
            {
                retValue = "<proot><error>-02</error><desc>" + ex.ToString() + "</desc></proot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #03
        public string getMedicoDatosExtra(string MedicoID)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_prpr_prov", MedicoID));                   //[0] 00003433
                lstParam.Add(sysConn.NewParmOut("@ps_prpr_correo", DbType.String));         //[1] drmcgarcia@hotmail.com
                lstParam.Add(sysConn.NewParmOut("@ps_prpr_cedula", DbType.String));         //[2] AECEM9999
                lstParam.Add(sysConn.NewParmOut("@ps_fecha_cedula_spec", DbType.DateTime)); //[3] 2010-01-01 00:00:00
                lstParam.Add(sysConn.NewParmOut("@pi_prpr_cedula_prof", DbType.String));   //[4] 1000
                lstParam.Add(sysConn.NewParmOut("@ps_fecha_cedula_prof", DbType.DateTime)); //[5] 1998-12-25 00:00:00
                lstParam.Add(sysConn.NewParmOut("@ps_casa_estudio", DbType.String));        //[6] UNI

                sysConn.sybaseExecuteQuery("dbo.sps_aud_busca_correo_prov", lstParam);

                string fecha_cedula_pro = getFormatoFecha(lstParam[5].Value.ToString(), false);
                string fecha_cedula_espec = getFormatoFecha(lstParam[3].Value.ToString(), false);

                retValue = "<proot>" +
                    "<error>00</error><desc></desc>" +
                    "<email>" + lstParam[1].Value.ToString() + "</email>" +
                    "<cedula_pro>" + lstParam[4].Value.ToString() + "</cedula_pro>" +
                    "<fecha_cedula_pro>" + fecha_cedula_pro + "</fecha_cedula_pro>" +
                    "<cedula_espec>" + lstParam[2].Value.ToString() + "</cedula_espec>" +
                    "<fecha_cedula_espec>" + fecha_cedula_espec + "</fecha_cedula_espec>" +
                    "<casa_estudios>" + lstParam[6].Value.ToString() + "</casa_estudios>" +
                "</proot>";
            }
            catch (AseException sybex)
            {
                retValue = "<proot><error>-03</error><desc>" + sybex.ToString() + "</desc></proot>";
            }
            catch (Exception ex)
            {
                retValue = "<proot><error>-02</error><desc>" + ex.ToString() + "</desc></proot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #04
        public string setMedicoCorreo(string MedicoID, string CorreoElectronico, string Cedula, DateTime CedulaFecha, string CedulaEspecialidad, DateTime CedulaEspecialidadFecha, string CasaEstudios)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();
            
            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_prpr_prov", MedicoID));
                lstParam.Add(sysConn.NewParm("@ps_prpr_correo", CorreoElectronico));
                lstParam.Add(sysConn.NewParm("@ps_prpr_cedula_spec", CedulaEspecialidad));
                lstParam.Add(sysConn.NewParm("@ps_fecha_cedula_spec", CedulaEspecialidadFecha.ToString("yyyy-MM-dd")));
                lstParam.Add(sysConn.NewParm("@pi_prpr_cedula_prof", Cedula));
                lstParam.Add(sysConn.NewParm("@ps_fecha_cedula_prof", CedulaFecha.ToString("yyyy-MM-dd")));
                lstParam.Add(sysConn.NewParm("@ps_casa_estudio", CasaEstudios));
                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.spu_aud_actualiza_correo_prov", lstParam);
                
                retValue = "<proot><error>"+ lstParam[7].Value.ToString() + "</error><desc></desc></proot>";
            }
            catch (AseException sybex)
            {
                retValue = "<proot><error>-03</error><desc>" + sybex.ToString() + "</desc></proot>";
            }
            catch (Exception ex)
            {
                retValue = "<proot><error>-02</error><desc>" + ex.ToString() + "</desc></proot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #05
        public string getMedicoAcuerdoEspecial(string MedicoID, int Cliente)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sprpr", MedicoID));
                lstParam.Add(sysConn.NewParm("@igrupo", Cliente));
                lstParam.Add(sysConn.NewParmOut("@ps_estatus", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_aud_dental_acuerdo_espec", lstParam);
                
                retValue = "<medico>" +
                        "<error>00</error><desc></desc>" +
                        "<acuerdoEspecial>" + lstParam[2].Value.ToString() + "</acuerdoEspecial>" +
                    "</medico>";
            }
            catch (AseException sybex)
            {
                retValue = "<medico><error>-03</error><desc>" + sybex.ToString() + "</desc></medico>";
            }
            catch (Exception ex)
            {
                retValue = "<medico><error>-02</error><desc>" + ex.ToString() + "</desc></medico>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #06
        public string getMedicoValidaMasto(int Cliente, int CK)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();
            //<medico><error>00</error><desc></desc><validaMasto>00</validaMasto></medico>
            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@GRGR_CK", Cliente));
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPS_AUD_VAMA_VALIDA_MASTO", lstParam);

                retValue = "<medico>" +
                        "<error>00</error><desc></desc>" +
                        "<validaMasto>" + lstParam[2].Value.ToString() + "</validaMasto>" +
                    "</medico>";
            }
            catch (AseException sybex)
            {
                retValue = "<medico><error>-03</error><desc>" + sybex.ToString() + "</desc></medico>";
            }
            catch (Exception ex)
            {
                retValue = "<medico><error>-02</error><desc>" + ex.ToString() + "</desc></medico>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #07
        public string getMedicoValidaProsta(int Cliente, int CK)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();
            //<medico><error>00</error><desc></desc><validaAntripros>00</validaAntripros></medico>
            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@GRGR_CK", Cliente));
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPS_AUD_VALIDA_ANTIPROS", lstParam);

                retValue = "<medico>" +
                        "<error>00</error><desc></desc>" +
                        "<validaAntipros>" + lstParam[2].Value.ToString() + "</validaAntipros>" +
                    "</medico>";
            }
            catch (AseException sybex)
            {                           
                retValue = "<medico><error>-03</medico><desc>" + sybex.ToString() + "</desc></medico>";
            }
            catch (Exception ex)
            {
                retValue = "<medico><error>-02</error><desc>" + ex.ToString() + "</desc></medico>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Paciente
        
        //***************************************************************** Paciente
        [WebMethod] // #08 =/=/=/=/=/=/=/=/=/=/ DEVUELVE VARIOS REGISTROS =/=/=/=/=/=/=/=/=/=/
        public string getPacienteDatos(string Credencial, string Grupo, string Nomina, string Beneficiario)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();
            
            try
            {
                string _ClaveGrupo = "";
                string _Credencual = "";
                string _SubGrupo = "";
                string _Nomina = "";
                string _Beneficiario = "";
                string _PacienteNombre = "";
                string _PacienteApellidoPat = "";
                string _PacienteApellidoMat = "";
                string _Relacion = "";
                string _CK = "";
                string _Departamento = "";
                string _IMSS = "";
                string _PVH = "";
                string _FechaNacimiento = "";
                string _Estatus = "";
                string _FechaVigenciaIni = "";
                string _FechaVigenciaFin = "";
                string _GrupoPago = "";
                string _Plan = "";
                string _LifeStyle = "";
                string _PlanBNX = "";
                string _ClientePagador = "";
                string _Sexo = "";
                string _Edad = "";
                string _BupaMemberID = "";
                string _Jubilado = "";

                int iCK = 0;
                int iBeneficiario = 0;
                int.TryParse(Beneficiario, out iBeneficiario);

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@num_creden", Credencial));
                lstParam.Add(sysConn.NewParm("@grupo", Grupo));
                lstParam.Add(sysConn.NewParm("@nomina", Nomina));
                lstParam.Add(sysConn.NewParm("@benef", iBeneficiario));
                lstParam.Add(sysConn.NewParm("@nombre", ""));
                lstParam.Add(sysConn.NewParm("@apellido", ""));
                lstParam.Add(sysConn.NewParm("@imss", ""));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_portal_busca_benef", lstParam);

                retValue = "<ckroot><error>00</error><desc></desc>";

                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    string sCK = dtResult.Rows[i][9].ToString();

                    _ClaveGrupo += "|" + dtResult.Rows[i][0].ToString();
                    _Credencual += "|" + dtResult.Rows[i][1].ToString();
                    _SubGrupo += "|" + dtResult.Rows[i][2].ToString();
                    _Nomina += "|" + dtResult.Rows[i][3].ToString();
                    _Beneficiario += "|" + dtResult.Rows[i][4].ToString();
                    _PacienteNombre += "|" + dtResult.Rows[i][5].ToString();
                    _PacienteApellidoPat += "|" + dtResult.Rows[i][6].ToString();
                    _PacienteApellidoMat += "|" + dtResult.Rows[i][7].ToString();
                    _Relacion += "|" + dtResult.Rows[i][8].ToString();
                    _CK += "|" + sCK;
                    _Departamento += "|" + dtResult.Rows[i][10].ToString();
                    _IMSS += "|" + dtResult.Rows[i][11].ToString();
                    _PVH += "|" + dtResult.Rows[i][12].ToString();
                    _FechaNacimiento += "|" + getFormatoFecha(dtResult.Rows[i][13].ToString(), true);
                    _Estatus += "|" + dtResult.Rows[i][14].ToString();
                    _FechaVigenciaIni += "|" + getFormatoFecha(dtResult.Rows[i][15].ToString(), true);
                    _FechaVigenciaFin += "|" + getFormatoFecha(dtResult.Rows[i][16].ToString(), true);
                    _GrupoPago += "|" + dtResult.Rows[i][17].ToString();
                    _Plan += "|" + dtResult.Rows[i][18].ToString();
                    _LifeStyle += "|" + dtResult.Rows[i][19].ToString();
                    _PlanBNX += "|" + dtResult.Rows[i][20].ToString();
                    _BupaMemberID += "|" + dtResult.Rows[i][21].ToString();
                    _Jubilado += "|" + dtResult.Rows[i][22].ToString();

                    int.TryParse(sCK, out iCK); //Se convierte el CK a entero
                    _ClientePagador += "|" + getCK_data_(iCK, 0);
                    _Sexo += "|" + getCK_data_(iCK, 1);
                    _Edad += "|" + getCK_data_(iCK, 2);
                }

                retValue +=
                    "<ck>" + _CK + "</ck>" +
                    "<credencial>" + _Credencual + "</credencial>" +
                    "<subgrupo>" + _SubGrupo + "</subgrupo>" +
                    "<nomina>" + _Nomina + "</nomina>" +
                    "<beneficiario>" + _Beneficiario + "</beneficiario>" +
                    "<nombrePaciente>" + _PacienteNombre + "</nombrePaciente>" +
                    "<apellidoPaciente>" + _PacienteApellidoPat + "</apellidoPaciente>" +
                    "<apellidoMatPaciente>" + _PacienteApellidoMat + "</apellidoMatPaciente>" +
                    "<edad>" + _Edad + "</edad>" +
                    "<sexo>" + _Sexo + "</sexo>" +
                    "<claveGrupo>" + _ClaveGrupo + "</claveGrupo>" +
                    "<claveCliente>" + "" + "</claveCliente>" +
                    "<pagador>" + _ClientePagador + "</pagador>" +
                    "<imss>" + _IMSS + "</imss>" +
                    "<relacion>" + _Relacion + "</relacion>" +
                    "<dept>" + _Departamento + "</dept>" +
                    "<pvh>" + _PVH + "</pvh>" +
                    "<fechaNacimiento>" + _FechaNacimiento + "</fechaNacimiento>" +
                    "<estatus>" + _Estatus + "</estatus>" +
                    "<fechaVigIni>" + _FechaVigenciaIni + "</fechaVigIni>" +
                    "<fechaVigFin>" + _FechaVigenciaFin + "</fechaVigFin>" +
                    "<grupoPago>" + _GrupoPago + "</grupoPago>" +
                    "<plan>" + _Plan + "</plan>" +
                    "<lifeStyle>" + _LifeStyle + "</lifeStyle>" +
                    "<planBNX>" + _PlanBNX + "</planBNX>" +
                    "<memberIdBupa>" + _BupaMemberID + "</memberIdBupa>" +
                    "<jubilado>" + _Jubilado + "</jubilado>" +
                "</ckroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<ckroot><error>-03</error><desc>" + sybex.ToString() + "</desc></ckroot>";
            }
            catch (Exception ex)
            {
                retValue = "<ckroot><error>-02</error><desc>" + ex.ToString() + "</desc></ckroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        private string getCK_data_(int iCK, int op)
        {
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();
            List<AseParameter> lstParam = new List<AseParameter>();
            int p = 0;

            switch(op)
            {
                case 0: //************** Cliente Pagador
                    lstParam.Add(sysConn.NewParm("@sGRGR", ""));
                    lstParam.Add(sysConn.NewParm("@iMEME", iCK));
                    lstParam.Add(sysConn.NewParmOut("@sCLIENTE", DbType.String));
                    sysConn.sybaseExecuteQuery("dbo.sp_aud_cliente_pagador", lstParam);
                    p = 2;
                    break;
                case 1: //************** Genero
                    lstParam.Add(sysConn.NewParm("@iBENEF", iCK));
                    lstParam.Add(sysConn.NewParmOut("@sSexo", DbType.String));
                    lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                    sysConn.sybaseExecuteQuery("dbo.sp_aud_portal_con_genero", lstParam);
                    p = 1;
                    break;
                case 2: //************** Edad
                    lstParam.Add(sysConn.NewParm("@iBENEF", iCK));
                    lstParam.Add(sysConn.NewParmOut("@iEdad", DbType.Int32));
                    lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                    sysConn.sybaseExecuteQuery("dbo.sp_aud_portal_con_edad", lstParam);
                    p = 1;
                    break;
            }
            sysConn.closeConnection();

            return lstParam[p].Value.ToString().Trim();
        }

        [WebMethod] // #09
        public string setPacienteDatosExtra(int CK, int Cliente, string TelContacto, string TelEmergencia, string CorreoElectronico, string CodigoPostal)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();
            
            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParm("@GRGR_CK", Cliente));
                lstParam.Add(sysConn.NewParm("@TEL_EMERGENCIA", TelEmergencia));
                lstParam.Add(sysConn.NewParm("@TEL_CONTACTO", TelContacto));
                lstParam.Add(sysConn.NewParm("@CORREO", CorreoElectronico));
                lstParam.Add(sysConn.NewParm("@CP", CodigoPostal));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_DHB_ADICIONALES", lstParam);

                retValue = "<ckroot><error>" + lstParam[6].Value.ToString() + "</error><desc></desc></ckroot>";                
            }
            catch (AseException sybex)
            {
                retValue = "<ckroot><error>-03</error><desc>" + sybex.ToString() + "</desc></ckroot>";
            }
            catch (Exception ex)
            {
                retValue = "<ckroot><error>-02</error><desc>" + ex.ToString() + "</desc></ckroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Consulta
        //*****************************************************************
        //          Consulta
        //*****************************************************************

        [WebMethod] // #10
        public string getElegibilidad(string Credencial, string MedicoID, int TipoMedico, string Pase, int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pDescripcion = "";
                string _pCopago = "";
                string _pDeducible = "";
                string _pCoaseguro = "";
                string _pLimite = "";
                string _num_Autoriza = "";
                string _msg_retorno = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@crd_numero", Credencial));
                lstParam.Add(sysConn.NewParm("@cve_medico", MedicoID));
                lstParam.Add(sysConn.NewParm("@tipo_medico", TipoMedico));
                lstParam.Add(sysConn.NewParm("@num_pase", Pase));
                lstParam.Add(sysConn.NewParm("@host", "7"));

                lstParam.Add(sysConn.NewParmOut("@num_autoriza", DbType.String));
                lstParam.Add(sysConn.NewParmOut("@msg_retorno", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_audiomatico_01 ", lstParam);

                _num_Autoriza = lstParam[5].Value.ToString();
                _msg_retorno = lstParam[6].Value.ToString();

                //************** Cliente Pagador
                lstParam.Clear(); //Se limpian los parámetros para buscar otro valor                    
                lstParam.Add(sysConn.NewParm("@iBENEF", CK));
                lstParam.Add(sysConn.NewParmOut("@sCLIENTE", DbType.String));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_portal_con_beneficios", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _pDescripcion = dtResult.Rows[0][0].ToString();
                    _pCopago = dtResult.Rows[0][1].ToString();
                    _pDeducible = dtResult.Rows[0][2].ToString();
                    _pCoaseguro = dtResult.Rows[0][3].ToString();
                    _pLimite = dtResult.Rows[0][4].ToString();
                }

                retValue = "<elegroot>" +
                    "<auth>" + _num_Autoriza + "</auth>" +
                    "<error>" + _msg_retorno + "</error>" +
                    "<copagos>" + _pCopago + "</copagos>" +
                    "<descripcion>" + _pDescripcion.Trim() + "</descripcion>" +
                    "<deducible>" + _pDeducible + "</deducible>" +
                    "<coaseguro>" + _pCoaseguro + "</coaseguro>" +
                    "<limite>" + _pLimite + "</limite>" +
                    "<peticiones>0</peticiones>" +
                "</elegroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<elegroot><error>-03</error><desc>" + sybex.ToString() + "</desc></elegroot>";
            }
            catch (Exception ex)
            {
                retValue = "<elegroot><error>-02</error><desc>" + ex.ToString() + "</desc></elegroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #11
        public string setAudiomatico_02(string MedicoID, string Elegibilidad, string Preautorizacion, DateTime FechaServicio)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sAUTO", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@sPREA", Preautorizacion));
                lstParam.Add(sysConn.NewParm("@dFSER", FechaServicio));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_audiomatico_02", lstParam);

                retValue = "<ccroot><error>" + lstParam[4].Value.ToString() + "</error><storecall>02</storecall><numerorec></numerorec></ccroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<ccroot><error>-03</error><storecall>" + sybex.ToString() + "</storecall><numerorec></numerorec></ccroot>";
            }
            catch (Exception ex)
            {
                retValue = "<ccroot><error>-02</error><storecall>" + ex.ToString() + "</storecall><numerorec></numerorec></ccroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #12
        public string setAudiomatico_03(string MedicoID, string Elegibilidad, string ICD, string LugarServicio, string CPT)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sAUTO", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@sIDCD", ICD));
                lstParam.Add(sysConn.NewParm("@sLSER", LugarServicio));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_audiomatico_03", lstParam);

                retValue = "<ccroot><error>" + lstParam[5].Value.ToString() + "</error><storecall>03</storecall><numerorec></numerorec></ccroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<ccroot><error>-03</error><storecall>" + sybex.ToString() + "</storecall><numerorec></numerorec></ccroot>";
            }
            catch (Exception ex)
            {
                retValue = "<ccroot><error>-02</error><storecall>" + ex.ToString() + "</storecall><numerorec></numerorec></ccroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #13
        public string setAudiomatico_04(string Elegibilidad, string Host, string Pase, string IncapacidadTipo, string IncapacidadDias, DateTime IncapadidadFecha, string Folio)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sAUTO", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@sHOST", Host));
                lstParam.Add(sysConn.NewParm("@iEXPA", Pase));
                lstParam.Add(sysConn.NewParm("@sINCAPACIDAD", IncapacidadTipo));
                lstParam.Add(sysConn.NewParm("@sDIASINCA", IncapacidadDias));
                lstParam.Add(sysConn.NewParm("@dFECHAINCA", IncapadidadFecha));
                lstParam.Add(sysConn.NewParm("@sFOLIO", Folio));

                lstParam.Add(sysConn.NewParmOut("@sCLRC", DbType.String));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_audiomatico_04", lstParam);

                retValue = "<ccroot><error>" + lstParam[8].Value.ToString() + "</error><storecall>04</storecall><numerorec>" + lstParam[7].Value.ToString() + "</numerorec></ccroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<ccroot><error>-03</error><storecall>" + sybex.ToString() + "</storecall><numerorec></numerorec></ccroot>";
            }
            catch (Exception ex)
            {
                retValue = "<ccroot><error>-02</error><storecall>" + ex.ToString() + "</storecall><numerorec></numerorec></ccroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #14
        public string setMedicamentoPASO(string Reclamacion, string CodigoEAN, string Formato, int Unidades, int Dias, int Veces, int VecesPeriodo, int Duracion, int DuracionPeriodo,
           double CantidadToma, string Preautorizacion, string TipoReclamacion, string FolioReceta, string Presentacion, string FirmaDigital,
           string RecetaDesabasto, string CodigoDesabasto, string MotivoDesabasto)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_reclamacion", Reclamacion));
                lstParam.Add(sysConn.NewParm("@ps_codigo_ean", CodigoEAN));
                lstParam.Add(sysConn.NewParm("@ps_formato", Formato));
                lstParam.Add(sysConn.NewParm("@pi_unidades", Unidades));
                lstParam.Add(sysConn.NewParm("@pi_dias", Dias));
                lstParam.Add(sysConn.NewParm("@pi_veces", Veces));
                lstParam.Add(sysConn.NewParm("@pi_veces_per", VecesPeriodo));
                lstParam.Add(sysConn.NewParm("@pi_duracion", Duracion));
                lstParam.Add(sysConn.NewParm("@pi_dura_per", DuracionPeriodo));
                lstParam.Add(sysConn.NewParm("@pm_cant_toma", CantidadToma));
                lstParam.Add(sysConn.NewParm("@ps_preautoriza", Preautorizacion));
                lstParam.Add(sysConn.NewParm("@ps_tipo_recl", TipoReclamacion));
                lstParam.Add(sysConn.NewParm("@ps_folio_receta", FolioReceta));
                // 2021-008222 Pases Digitales
                lstParam.Add(sysConn.NewParm("@ps_presentacion", Presentacion));
                lstParam.Add(sysConn.NewParm("@ps_firma_digital", FirmaDigital));
                // REQ0020318 Desabasto
                lstParam.Add(sysConn.NewParm("@ps_receta_desabasto", RecetaDesabasto));
                lstParam.Add(sysConn.NewParm("@ps_codigo_ean_desabasto", CodigoDesabasto));
                lstParam.Add(sysConn.NewParm("@ps_motivo_desabasto", MotivoDesabasto));

                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_INSERTA_PASO_EAN", lstParam);

                retValue = "<cext><error>" + lstParam[18].Value.ToString() + "</error><desc></desc></cext>";
            }
            catch (AseException sybex)
            {
                retValue = "<cext><error>-03</error><desc>" + sybex.ToString() + "</desc></cext>";
            }
            catch (Exception ex)
            {
                retValue = "<cext><error>-02</error><desc>" + ex.ToString() + "</desc></cext>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #15
        public string setMedicamentoEAN(string Reclamacion)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_reclamacion", Reclamacion));        
                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_INSERTA_MED_EAN", lstParam);

                retValue = "<cext><error>" + lstParam[1].Value.ToString() + "</error><desc></desc></cext>";
            }
            catch (AseException sybex)
            {                
                retValue = "<cext><error>-03</error><desc>" + sybex.ToString() + "</desc></cext>";
            }
            catch (Exception ex)
            {
                retValue = "<cext><error>-02</error><desc>" + ex.ToString() + "</desc></cext>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #16
        public string setServicioPaso(string Reclamacion, int Consecutivo, string Formato, string TipoServicio, string Especialidad, string Preautorizacion, string CPT, int Unidades, 
            double OjoDer_1, double OjoIzq_1, double OjoDer_2, double OjoIzq_2, string Diagnostico, string Generado, string TipoReclamacion, string FolioReceta, string FirmaDigital)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_reclamacion", Reclamacion));
                lstParam.Add(sysConn.NewParm("@pi_consecutivo", Consecutivo));
                lstParam.Add(sysConn.NewParm("@ps_formato", Formato));
                lstParam.Add(sysConn.NewParm("@pi_tipo_serv", TipoServicio));
                lstParam.Add(sysConn.NewParm("@ps_espec", Especialidad));
                lstParam.Add(sysConn.NewParm("@ps_preaut", Preautorizacion));
                lstParam.Add(sysConn.NewParm("@ps_cpt", CPT));
                lstParam.Add(sysConn.NewParm("@pi_unidades", Unidades));
                lstParam.Add(sysConn.NewParm("@ps_ojo_dere_1", OjoDer_1));
                lstParam.Add(sysConn.NewParm("@ps_ojo_izq_1", OjoIzq_1));
                lstParam.Add(sysConn.NewParm("@ps_ojo_dere_2", OjoDer_2));
                lstParam.Add(sysConn.NewParm("@ps_ojo_izq_2", OjoIzq_2));
                lstParam.Add(sysConn.NewParm("@ps_diagnostico", Diagnostico));
                lstParam.Add(sysConn.NewParm("@ps_generado", Generado));
                lstParam.Add(sysConn.NewParm("@ps_tipo_recl", TipoReclamacion));
                // 2021-008222 Pases Digitales
                lstParam.Add(sysConn.NewParm("@ps_folio", FolioReceta));
                lstParam.Add(sysConn.NewParm("@ps_firma_digital", FirmaDigital));

                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_INSERTA_PASO_SERVICIO", lstParam);

                retValue = "<cext><error>" + lstParam[17].Value.ToString() + "</error><desc></desc></cext>";
            }
            catch (AseException sybex)
            {
                retValue = "<cext><error>-03</error><desc>" + sybex.ToString() + "</desc></cext>";
            }
            catch (Exception ex)
            {
                retValue = "<cext><error>-02</error><desc>" + ex.ToString() + "</desc></cext>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #17
        public string setServicioTipo(string Reclamacion)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_reclamacion", Reclamacion));
                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_INSERTA_TIPO_SERVICIO", lstParam);

                retValue = "<cext><error>" + lstParam[1].Value.ToString() + "</error><desc></desc></cext>";
            }
            catch (AseException sybex)
            {
                retValue = "<cext><error>-03</error><desc>" + sybex.ToString() + "</desc></cext>";
            }
            catch (Exception ex)
            {
                retValue = "<cext><error>-02</error><desc>" + ex.ToString() + "</desc></cext>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] //  #18
        public string setConsultaCancelar(string Elegibilidad, string MedicoID)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_eleg", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@ps_prpr", MedicoID));

                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sps_aud_valida_eleg_portal", lstParam);

                retValue = "<elegroot><error>" + lstParam[2].Value.ToString() + "</error><desc></desc></elegroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<elegroot><error>-03</error><desc>" + sybex.ToString() + "</desc></elegroot>";
            }
            catch (Exception ex)
            {
                retValue = "<elegroot><error>-02</error><desc>" + ex.ToString() + "</desc></elegroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #44
        public string ValidaElegibilidadEspecial(string Elegibilidad, int CK, string MedicoID)

        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sELEG", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPS_AUD_VALIDA_ELEG_MPC ", lstParam);

                retValue = "<elegroot><error>" + lstParam[3].Value.ToString() + "</error><desc></desc></elegroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<elegroot><error>-03</error><desc>" + sybex.ToString() + "</desc></elegroot>";
            }
            catch (Exception ex)
            {
                retValue = "<elegroot><error>-02</error><desc>" + ex.ToString() + "</desc></elegroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #47
        public string setProfilingData(string Reclamacion, int Consecutivo, int Edad, double Tiempo, string Visita, string Servicio, int Unidades, int PiezasCaja, 
            double Peso, double Estatura, string Presion, double Glucosa, double IMC, double Temperatura, double Perimetro,
            double FrecuenciaCardiaca, double FrecuenciaRespiratoria, double Hemoglobina, double OsteoporosisMayor, double FracturaCadera, double Densitometria, 
            string Urgencia, string iTipo, string iSubTipo, int iDias, DateTime iFecha, string DiaSemana, string Receta, string Huella, DateTime FechaConsulta)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@CLCL_ID", Reclamacion));
                lstParam.Add(sysConn.NewParm("@CDML_SEQ_NO", Consecutivo));
                lstParam.Add(sysConn.NewParm("@EDAD", Edad));
                lstParam.Add(sysConn.NewParm("@TIEMPO", Tiempo));
                lstParam.Add(sysConn.NewParm("@VISITA", Visita));
                lstParam.Add(sysConn.NewParm("@PROCEDIEMIENTO", Servicio));
                lstParam.Add(sysConn.NewParm("@UNIDADES", Unidades));
                lstParam.Add(sysConn.NewParm("@PIEZAS_X_CAJA", PiezasCaja));
                lstParam.Add(sysConn.NewParm("@PESO", Peso));
                lstParam.Add(sysConn.NewParm("@ESTATURA", Estatura));
                lstParam.Add(sysConn.NewParm("@PRESION_ART", Presion));
                lstParam.Add(sysConn.NewParm("@GLUCOSA", Glucosa));
                lstParam.Add(sysConn.NewParm("@IMC", IMC));
                lstParam.Add(sysConn.NewParm("@TEMPERATURA", Temperatura));
                lstParam.Add(sysConn.NewParm("@PERIMETRO_ABDO", Perimetro));
                lstParam.Add(sysConn.NewParm("@FREC_CARDIACA", FrecuenciaCardiaca));
                lstParam.Add(sysConn.NewParm("@FREC_RESPIRA", FrecuenciaRespiratoria));
                lstParam.Add(sysConn.NewParm("@HEMOGLOBINA", Hemoglobina));
                lstParam.Add(sysConn.NewParm("@OSTEOPORISIS", OsteoporosisMayor));
                lstParam.Add(sysConn.NewParm("@FRACTURA_CADERA", FracturaCadera));
                lstParam.Add(sysConn.NewParm("@DENSITOMETRIA", Densitometria));
                lstParam.Add(sysConn.NewParm("@URGENCIA", Urgencia));
                lstParam.Add(sysConn.NewParm("@TIPO_INCAP", iTipo));
                lstParam.Add(sysConn.NewParm("@SUBTIPO_INCAP", iSubTipo));
                lstParam.Add(sysConn.NewParm("@DIAS_INCAP", iDias));
                lstParam.Add(sysConn.NewParm("@FECHA_INICIO", iFecha));
                lstParam.Add(sysConn.NewParm("@DIA_SEMANA", DiaSemana));
                lstParam.Add(sysConn.NewParm("@FORMATO", Receta));
                lstParam.Add(sysConn.NewParm("@OMISION_HUELLA", Huella));
                lstParam.Add(sysConn.NewParm("@FECHA_SERVICIO", FechaConsulta));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_PRO_DESC_CE", lstParam);

                retValue = "<root><error>" + lstParam[30].Value.ToString() + "</error><errordesc></errordesc></root>";
            }
            catch (AseException sybex)
            {
                retValue = "<root><error>-03</error><errordesc>" + sybex.ToString() + "</errordesc></root>";
            }
            catch (Exception ex)
            {
                retValue = "<root><error>-02</error><errordesc>" + ex.ToString() + "</errordesc></root>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        
        [WebMethod] // #48
        public string getBUPAValidaICDCPT(int CK, string ICD, string CPT, int Ingreso)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));                   // [0]
                lstParam.Add(sysConn.NewParm("@IDCD_ID", ICD));                  // [1]
                lstParam.Add(sysConn.NewParm("@IPCD_ID", CPT));                  // [2]
                lstParam.Add(sysConn.NewParm("@IMP_INGRESO", Ingreso));          // [3]
                lstParam.Add(sysConn.NewParmOut("@msg_retorno", DbType.String)); // [4]
                lstParam.Add(sysConn.NewParmOut("@desc_msg", DbType.String));    // [5]

                sysConn.sybaseExecuteQuery("dbo.SP_BUPA_VALIDA_CREAR_ELEG", lstParam);

                retValue = "<bupa>" +
                    "<error>" + lstParam[4].Value.ToString() + "</error>" +
                    "<desc>" + lstParam[5].Value.ToString() + "</desc>" +
                "</bupa>";
            }
            catch (AseException sybex)
            {
                retValue = "<bupa><error>-03</error><desc>" + sybex.ToString() + "</desc></bupa>";
            }
            catch (Exception ex)
            {
                retValue = "<bupa><error>-02</error><desc>" + ex.ToString() + "</desc></bupa>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #49
        public string getBUPAValidaDXTX(int CK, string Tipo, string ClaveDxTx)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));      // [0]
                lstParam.Add(sysConn.NewParm("@DXTX", Tipo));       // [1]
                lstParam.Add(sysConn.NewParm("@CD_ID", ClaveDxTx)); // [2]

                dtResult = sysConn.sybaseGetTable("dbo.SP_BUPA_VALIDA_DX_TX", lstParam);

                string vError = dtResult.Rows[0][0].ToString();
                string vServicio = dtResult.Rows[0][1].ToString();
                string vPeriodo = dtResult.Rows[0][2].ToString();
                string vContenido1 = dtResult.Rows[0][3].ToString();
                string vContenido2 = dtResult.Rows[0][4].ToString();
                string vContenido3 = dtResult.Rows[0][5].ToString();
                string vContenido4 = dtResult.Rows[0][6].ToString();

                retValue = "<bupa>" +
                    "<error>" + vError + "</error>" +
                    "<serv>" + vServicio + "</serv>" +
                    "<periodo>" + vPeriodo + "</periodo>" +
                    "<contenido1>" + vContenido1 + "</contenido1>" +
                    "<contenido2>" + vContenido2 + "</contenido2>" +
                    "<contenido3>" + vContenido3 + "</contenido3>" +
                    "<contenido4>" + vContenido4 + "</contenido4>" +
                "</bupa>";
            }
            catch (AseException sybex)
            {
                retValue = "<bupa><error>-03</error><desc>" + sybex.ToString() + "</desc></bupa>";
            }
            catch (Exception ex)
            {
                retValue = "<bupa><error>-02</error><desc>" + ex.ToString() + "</desc></bupa>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #50
        public string getBUPANoIdentificador(int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                dtResult = sysConn.sybaseGetTable("dbo.SP_BUPA_ASAS_CID", lstParam);

                string vError = "";
                string vResult = "";

                if(dtResult.Rows.Count > 0)
                {
                    vError = "00";
                    vResult = dtResult.Rows[0][0].ToString();
                }
                else
                {
                    vError = "01";
                    vResult = "";
                }

                retValue = "<bupa><error>"+ vError + "</error><desc>" + vResult + "</desc></bupa>";
            }
            catch (AseException sybex)
            {
                retValue = "<bupa><error>-03</error><desc>" + sybex.ToString() + "</desc></bupa>";
            }
            catch (Exception ex)
            {
                retValue = "<bupa><error>-02</error><desc>" + ex.ToString() + "</desc></bupa>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #51
        public string getHistoricoCK(int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@VINT_MEME_CK", CK));
                lstParam.Add(sysConn.NewParmOut("@VCH_ERROR", DbType.String));
                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_MEME_HIST", lstParam);

                string vError = "00";
                string vResult = "";

                if (dtResult.Rows.Count > 0)
                {
                    foreach(DataRow dr in dtResult.Rows)
                    {
                        vResult += dr[0].ToString() + ",";
                    }

                    if (vResult.Length > 0)
                        vResult = vResult.Substring(0, vResult.Length - 1);
                }
                else
                {
                    vError = "01";
                }
                
                retValue = "<vita><error>"+ vError + "</error><desc>" + vResult + "</desc></vita>";
            }
            catch (AseException sybex)
            {
                retValue = "<vita><error>-03</error><desc>" + sybex.ToString() + "</desc></vita>";
            }
            catch (Exception ex)
            {
                retValue = "<vita><error>-02</error><desc>" + ex.ToString() + "</desc></vita>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #53 =/=/=/=/=/=/=/=/=/=/ DEVUELVE VARIOS REGISTROS =/=/=/=/=/=/=/=/=/=/
        public string getPacienteDatosHistorico(string Credencial, string Grupo, string Nomina, string Beneficiario)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _ClaveGrupo = "";
                string _Credencual = "";
                string _SubGrupo = "";
                string _Nomina = "";
                string _Beneficiario = "";
                string _PacienteNombre = "";
                string _PacienteApellidoPat = "";
                string _PacienteApellidoMat = "";
                string _Relacion = "";
                string _CK = "";
                string _Departamento = "";
                string _IMSS = "";
                string _PVH = "";
                string _FechaNacimiento = "";
                string _Estatus = "";
                string _FechaVigenciaIni = "";
                string _FechaVigenciaFin = "";
                string _GrupoPago = "";
                string _Plan = "";
                string _LifeStyle = "";
                string _PlanBNX = "";
                string _ClientePagador = "";
                string _Sexo = "";
                string _Edad = "";
                string _BupaMemberID = "";
                string _Jubilado = "";

                int iCK = 0;
                int iBeneficiario = 0;
                int.TryParse(Beneficiario, out iBeneficiario);

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@num_creden", Credencial));
                lstParam.Add(sysConn.NewParm("@grupo", Grupo));
                lstParam.Add(sysConn.NewParm("@nomina", Nomina));
                lstParam.Add(sysConn.NewParm("@benef", iBeneficiario));
                lstParam.Add(sysConn.NewParm("@nombre", ""));
                lstParam.Add(sysConn.NewParm("@apellido", ""));
                lstParam.Add(sysConn.NewParm("@imss", ""));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_busca_benef_hist", lstParam);

                retValue = "<ckroot><error>00</error><desc></desc>";

                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    string sCK = dtResult.Rows[i][9].ToString();

                    _ClaveGrupo += "|" + dtResult.Rows[i][0].ToString();
                    _Credencual += "|" + dtResult.Rows[i][1].ToString();
                    _SubGrupo += "|" + dtResult.Rows[i][2].ToString();
                    _Nomina += "|" + dtResult.Rows[i][3].ToString();
                    _Beneficiario += "|" + dtResult.Rows[i][4].ToString();
                    _PacienteNombre += "|" + dtResult.Rows[i][5].ToString();
                    _PacienteApellidoPat += "|" + dtResult.Rows[i][6].ToString();
                    _PacienteApellidoMat += "|" + dtResult.Rows[i][7].ToString();
                    _Relacion += "|" + dtResult.Rows[i][8].ToString();
                    _CK += "|" + sCK;
                    _Departamento += "|" + dtResult.Rows[i][10].ToString();
                    _IMSS += "|" + dtResult.Rows[i][11].ToString();
                    _PVH += "|" + dtResult.Rows[i][12].ToString();
                    _FechaNacimiento += "|" + getFormatoFecha(dtResult.Rows[i][13].ToString(), true);
                    _Estatus += "|" + dtResult.Rows[i][14].ToString();
                    _FechaVigenciaIni += "|" + getFormatoFecha(dtResult.Rows[i][15].ToString(), true);
                    _FechaVigenciaFin += "|" + getFormatoFecha(dtResult.Rows[i][16].ToString(), true);
                    _GrupoPago += "|" + dtResult.Rows[i][17].ToString();
                    _Plan += "|" + dtResult.Rows[i][18].ToString();
                    _LifeStyle += "|" + dtResult.Rows[i][19].ToString();
                    _PlanBNX += "|" + dtResult.Rows[i][20].ToString();
                    _BupaMemberID += "|" + dtResult.Rows[i][21].ToString();
                    _Jubilado += "|" + dtResult.Rows[i][22].ToString();

                    int.TryParse(sCK, out iCK); //Se convierte el CK a entero
                    _ClientePagador += "|" + getCK_data_(iCK, 0);
                    _Sexo += "|" + getCK_data_(iCK, 1);
                    _Edad += "|" + getCK_data_(iCK, 2);
                }

                retValue +=
                    "<ck>" + _CK + "</ck>" +
                    "<credencial>" + _Credencual + "</credencial>" +
                    "<subgrupo>" + _SubGrupo + "</subgrupo>" +
                    "<nomina>" + _Nomina + "</nomina>" +
                    "<beneficiario>" + _Beneficiario + "</beneficiario>" +
                    "<nombrePaciente>" + _PacienteNombre + "</nombrePaciente>" +
                    "<apellidoPaciente>" + _PacienteApellidoPat + "</apellidoPaciente>" +
                    "<apellidoMatPaciente>" + _PacienteApellidoMat + "</apellidoMatPaciente>" +
                    "<edad>" + _Edad + "</edad>" +
                    "<sexo>" + _Sexo + "</sexo>" +
                    "<claveGrupo>" + _ClaveGrupo + "</claveGrupo>" +
                    "<claveCliente>" + "" + "</claveCliente>" +
                    "<pagador>" + _ClientePagador + "</pagador>" +
                    "<imss>" + _IMSS + "</imss>" +
                    "<relacion>" + _Relacion + "</relacion>" +
                    "<dept>" + _Departamento + "</dept>" +
                    "<pvh>" + _PVH + "</pvh>" +
                    "<fechaNacimiento>" + _FechaNacimiento + "</fechaNacimiento>" +
                    "<estatus>" + _Estatus + "</estatus>" +
                    "<fechaVigIni>" + _FechaVigenciaIni + "</fechaVigIni>" +
                    "<fechaVigFin>" + _FechaVigenciaFin + "</fechaVigFin>" +
                    "<grupoPago>" + _GrupoPago + "</grupoPago>" +
                    "<plan>" + _Plan + "</plan>" +
                    "<lifeStyle>" + _LifeStyle + "</lifeStyle>" +
                    "<planBNX>" + _PlanBNX + "</planBNX>" +
                    "<memberIdBupa>" + _BupaMemberID + "</memberIdBupa>" +
                    "<jubilado>" + _Jubilado + "</jubilado>" +
                "</ckroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<ckroot><error>-03</error><desc>" + sybex.ToString() + "</desc></ckroot>";
            }
            catch (Exception ex)
            {
                retValue = "<ckroot><error>-02</error><desc>" + ex.ToString() + "</desc></ckroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #52
        public string getValidacionesBupa(string Grupo, string Nomina, int Benef, string MedicoID)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            { 
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@GRGR_ID", Grupo));
                lstParam.Add(sysConn.NewParm("@SBSB_ID", Nomina));
                lstParam.Add(sysConn.NewParm("@MEME_SFX", Benef));
                lstParam.Add(sysConn.NewParm("@PRPR_ID", MedicoID));
                dtResult = sysConn.sybaseGetTable("dbo.SP_BUPA_MENSAJE", lstParam);

                string Error = "";
                string ErrorDesc = "";
                string Clave = "";
                string Mensaje = "";
                string Accion = "";

                if(dtResult.Rows.Count > 0)
                {
                    Error = "00";
                    ErrorDesc = "";
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        Clave += dtResult.Rows[i][0].ToString() + "|";
                        Mensaje += dtResult.Rows[i][1].ToString() + "|";
                        Accion += dtResult.Rows[i][2].ToString() + "|";
                    }
                }
                else
                {
                    Error = "79";
                    ErrorDesc = "Tabla vacia";
                }

                retValue += 
                    "<bupa>" +
                        "<error>" + Error + "</error>" +
                        "<desc>" + ErrorDesc + "</desc>" +
                        "<clave>" + Clave + "</clave>" +
                        "<mensaje>" + Mensaje + "</mensaje>" +
                        "<accion>" + Accion + "</accion>" +
                    "</bupa>";
            }
            catch (AseException sybex)
            {
                retValue = "<bupa><error>-03</error><desc>" + sybex.ToString() + "</desc></bupa>";
            }
            catch (Exception ex)
            {
                retValue = "<bupa><error>-02</error><desc>" + ex.ToString() + "</desc></bupa>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #10
        public string ValidarElegibilidad(string Credencial, string MedicoID, int TipoMedico, string Pase)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@crd_numero", Credencial));
                lstParam.Add(sysConn.NewParm("@cve_medico", MedicoID));
                lstParam.Add(sysConn.NewParm("@tipo_medico", TipoMedico));
                lstParam.Add(sysConn.NewParm("@num_pase", Pase));
                lstParam.Add(sysConn.NewParm("@host", "7"));

                //lstParam.Add(sysConn.NewParmOut("@num_autoriza", DbType.String));
                lstParam.Add(sysConn.NewParmOut("@msg_retorno", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SP_AUD_VALIDACIONES_ELEG", lstParam);

                //_num_Autoriza = lstParam[5].Value.ToString();
                string _msg_retorno = lstParam[5].Value.ToString();

                retValue = "<valeleg><error>" + _msg_retorno + "</error><desc></desc></valeleg>";
            }
            catch (AseException sybex)
            {
                retValue = "<valeleg><error>-03</error><desc>" + sybex.ToString() + "</desc></valeleg>";
            }
            catch (Exception ex)
            {
                retValue = "<valeleg><error>-02</error><desc>" + ex.ToString() + "</desc></valeleg>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Copago
        //*****************************************************************
        //          Copago
        //*****************************************************************

        [WebMethod] // #19
        public string getCopagoProcedimientoMedico(int CK, DateTime FechaConsulta, string CPT, string LugarServicio, string MedicoID)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _cpt = "";
                string _pCosto = "";
                string _pPorciento = "";
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@dFSER", FechaConsulta));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));
                lstParam.Add(sysConn.NewParm("@sLSER", LugarServicio));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_OBTIENE_COPAGO", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _cpt = dtResult.Rows[0][0].ToString();
                    _pCosto = dtResult.Rows[0][1].ToString();
                    _pPorciento = dtResult.Rows[0][2].ToString();
                }

                retValue = "<copago>" +
                    "<errorcode>" + lstParam[5].Value.ToString() + "</errorcode>" +
                    "<cpt>" + _cpt + "</cpt>" +
                    "<costo>" + _pCosto + "</costo>" +
                    "<porciento>" + _pPorciento + "</porciento>" +
                    "</copago>";
            }
            catch (AseException sybex)
            {
                retValue = "<copago><errorcode>-03</errorcode><desc>" + sybex.ToString() + "</desc></copago>";

            }
            catch (Exception ex)
            {
                retValue = "<copago><errorcode>-02</errorcode><desc>" + ex.ToString() + "</desc></copago>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #20
        public string getCopagoMedicamento(string Grupo, string Plan)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pCosto = "";
                string _pPorciento = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sgrgr", Grupo));
                lstParam.Add(sysConn.NewParm("@splan", Plan));

                lstParam.Add(sysConn.NewParmOut("@ps_cod_err", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_copago_medicamento", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    // Respuesta query: [0] 20 | [1] %
                    _pPorciento = dtResult.Rows[0][0].ToString();   // 20
                    _pCosto = dtResult.Rows[0][1].ToString();       // %
                }
                retValue = "<copago>" +
                    "<errorcode>" + lstParam[2].Value.ToString() + "</errorcode>" +
                    "<costo>" + _pCosto + "</costo>" +
                    "<porciento>" + _pPorciento + "</porciento>" +
                    "</copago>";
            }
            catch (AseException sybex)
            {
                retValue = "<copago><errorcode>-03</errorcode><desc>" + sybex.ToString() + "</desc></copago>";
            }
            catch (Exception ex)
            {
                retValue = "<copago><errorcode>-02</errorcode><desc>" + ex.ToString() + "</desc></copago>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #21
        public string getCopagoReferido(int CK, DateTime FechaConsulta, string CPT, string LugarServicio)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _cpt = "";
                string _pCosto = "";
                string _pPorciento = "";
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@dFSER", FechaConsulta));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));
                lstParam.Add(sysConn.NewParm("@sLSER", LugarServicio));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_OBTIENE_COPAGO_REF", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _cpt = dtResult.Rows[0][0].ToString();
                    _pCosto = dtResult.Rows[0][1].ToString();
                    _pPorciento = dtResult.Rows[0][2].ToString();
                }
                retValue = "<copago>" +
                    "<errorcode>" + lstParam[4].Value.ToString() + "</errorcode>" +
                    "<cpt>" + _cpt + "</cpt>" +
                    "<costo>" + _pCosto + "</costo>" +
                    "<porciento>" + _pPorciento + "</porciento>" +
                    "</copago>";
            }
            catch (AseException sybex)
            {
                retValue = "<copago><errorcode>-03</errorcode><desc>" + sybex.ToString() + "</desc></copago>";

            }
            catch (Exception ex)
            {
                retValue = "<copago><errorcode>-02</errorcode><desc>" + ex.ToString() + "</desc></copago>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #22
        public string getCopagoTratamientoDental(int CK, DateTime FechaConsulta, string CPT, string LugarServicio, string MedicoID)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _cdt = "";
                string _pCosto = "";
                string _pPorciento = "";
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@dFSER", FechaConsulta));
                lstParam.Add(sysConn.NewParm("@sDPDP", CPT));
                lstParam.Add(sysConn.NewParm("@sLSER", LugarServicio));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_OBTIENE_COPAGO_DENTAL", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _cdt = dtResult.Rows[0][1].ToString();
                    _pCosto = dtResult.Rows[0][1].ToString();
                    _pPorciento = dtResult.Rows[0][2].ToString();
                }

                retValue = "<copago>" +
                    "<errorcode>" + lstParam[5].Value.ToString() + "</errorcode>" +
                    "<cdt>" + _cdt + "</cdt>" +
                    "<costo>" + _pCosto + "</costo>" +
                    "<porciento>" + _pPorciento + "</porciento>" +
                    "</copago>";
            }
            catch (AseException sybex)
            {
                retValue = "<copago><errorcode>-03</errorcode><desc>" + sybex.ToString() + "</desc></copago>";

            }
            catch (Exception ex)
            {
                retValue = "<copago><errorcode>-02</errorcode><desc>" + ex.ToString() + "</desc></copago>";

            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Remesa
        //***************************************************************** Remesa

        [WebMethod] // #23
        public string setRemesa(string Dummy, int Cliente)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pVitaError = "";
                string _pReclamacion = "";
                string _pNomina = "";
                string _pBeneficiario = "";
                string _pNombre = "";
                string _pFecha = "";
                string _pFechaServ = "";
                string _pElegibilidad = "";
                string _pDiagnostico = "";
                string _pMontoNoImp = "";
                string _pISR = "";
                string _pIVA = "";
                string _pIVARet = "";
                string _pTotal = "";
                string _pRemesa = "";
                string _pCopago = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@ps_prpr", Dummy));
                lstParam.Add(sysConn.NewParm("@pi_grupo", Cliente));
                dtResult = sysConn.sybaseGetTable("dbo.sps_aud_rep_remesa_asig", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _pVitaError = dtResult.Rows[0][0].ToString();
                    retValue = "<rroot><error>00</error><ve>" + _pVitaError + "</ve>";

                    if (dtResult.Columns.Count > 2)
                    {
                        for (int i = 0; i < dtResult.Rows.Count; i++)
                        {
                            _pReclamacion += "|" + dtResult.Rows[i][2].ToString();
                            _pNomina += "|" + dtResult.Rows[i][3].ToString();
                            _pBeneficiario += "|" + dtResult.Rows[i][4].ToString();
                            _pNombre += "|" + dtResult.Rows[i][5].ToString();
                            _pFecha += "|" + getFormatoFecha(dtResult.Rows[i][6].ToString(), false); //dtResult.Rows[i][6].ToString();
                            _pFechaServ += "|" + getFormatoFecha(dtResult.Rows[i][7].ToString(), false); //dtResult.Rows[i][7].ToString();
                            _pElegibilidad += "|" + dtResult.Rows[i][8].ToString();
                            _pDiagnostico += "|" + dtResult.Rows[i][9].ToString();
                            _pMontoNoImp += "|" + dtResult.Rows[i][10].ToString();
                            _pISR += "|" + dtResult.Rows[i][11].ToString();
                            _pIVA += "|" + dtResult.Rows[i][12].ToString();
                            _pIVARet += "|" + dtResult.Rows[i][13].ToString();
                            _pTotal += "|" + dtResult.Rows[i][14].ToString();
                            _pRemesa += "|" + dtResult.Rows[i][16].ToString();
                            _pCopago += "|" + dtResult.Rows[i][17].ToString();
                        }
                    }

                    retValue +=
                        "<rec>" + _pReclamacion + "</rec>" +
                        "<no>" + _pNomina + "</no>" +
                        "<be>" + _pBeneficiario + "</be>" +
                        "<nb>" + _pNombre + "</nb>" +
                        "<fh>" + _pFecha + "</fh>" +
                        "<fhs>" + _pFechaServ + "</fhs>" +
                        "<el>" + _pElegibilidad + "</el>" +
                        "<di>" + _pDiagnostico + "</di>" +
                        "<msi>" + _pMontoNoImp + "</msi>" +
                        "<isr>" + _pISR + "</isr>" +
                        "<iva>" + _pIVA + "</iva>" +
                        "<ivar>" + _pIVARet + "</ivar>" +
                        "<tot>" + _pTotal + "</tot>" +
                        "<re>" + _pRemesa + "</re>" +
                        "<copago>" + _pCopago + "</copago>" +
                    "</rroot>";
                }
                else
                {
                    retValue = "<rroot><error>-01</error><desc>Tabla sin registros</desc></rroot>";
                }
            }
            catch (AseException sybex)
            {
                retValue = "<rroot><error>-03</error><desc>" + sybex.ToString() + "</desc></rroot>";
            }
            catch (Exception ex)
            {
                retValue = "<rroot><error>-02</error><desc>" + ex.ToString() + "</desc></rroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #24
        public string getReclamacionDetalle(string Referencia, string Remesa, string Reclamacion, string MedicoID, DateTime FechaIni, DateTime FechaFin)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pReclamacion = "";
                string _pGrupo = "";
                string _pNomina = "";
                string _pBeneficiario = "";
                string _pPaciente = "";
                string _pFechaConsulta = "";
                string _pFechaPago = "";
                string _pEstatus = "";
                string _pOrgien = "";
                string _pCPT = "";
                string _pMonto = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sREFERENCIA", Referencia));
                lstParam.Add(sysConn.NewParm("@sNO_REMESA", Remesa));
                lstParam.Add(sysConn.NewParm("@sNO_CLRC", Reclamacion));
                lstParam.Add(sysConn.NewParm("@sPRPR_ID", MedicoID));
                lstParam.Add(sysConn.NewParm("@dFECHA_INI", FechaIni));
                lstParam.Add(sysConn.NewParm("@dFECHA_FIN", FechaFin));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_portal_aclara_pagos", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        _pReclamacion += "|" + dtResult.Rows[i][0].ToString();
                        _pGrupo += "|" + dtResult.Rows[i][1].ToString();
                        _pNomina += "|" + dtResult.Rows[i][2].ToString();
                        _pBeneficiario += "|" + dtResult.Rows[i][3].ToString();
                        _pPaciente += "|" + dtResult.Rows[i][4].ToString();
                        _pFechaConsulta += "|" + getFormatoFecha(dtResult.Rows[i][5].ToString(), false); // dtResult.Rows[i][5].ToString();
                        _pFechaPago += "|" + getFormatoFecha(dtResult.Rows[i][6].ToString(), false); // dtResult.Rows[i][6].ToString();
                        _pEstatus += "|" + dtResult.Rows[i][7].ToString();
                        _pOrgien += "|" + dtResult.Rows[i][8].ToString();
                        _pCPT += "|" + dtResult.Rows[i][9].ToString();
                        _pMonto += "|" + dtResult.Rows[i][11].ToString();
                    }
                }

                retValue = "<stroot>" +
                            "<rec>" + _pReclamacion + "</rec>" +
                            "<gr>" + _pGrupo + "</gr>" +
                            "<no>" + _pNomina + "</no>" +
                            "<be>" + _pBeneficiario + "</be>" +
                            "<pa>" + _pPaciente + "</pa>" +
                            "<fc>" + _pFechaConsulta + "</fc>" +
                            "<fp>" + _pFechaPago + "</fp>" +
                            "<st>" + _pEstatus + "</st>" +
                            "<or>" + _pOrgien + "</or>" +
                            "<cp>" + _pCPT + "</cp>" +
                            "<mn>" + _pMonto + "</mn>" +
                            "<error>" + lstParam[6].Value.ToString() + "</error>" +
                    "</stroot>";
            }
            catch (AseException sybex)
            {
                retValue = "<stroot><error>-03</error><desc>" + sybex.ToString() + "</desc></stroot>";
            }
            catch (Exception ex)
            {
                retValue = "<stroot><error>-02</error><desc>" + ex.ToString() + "</desc></stroot>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #25
        public string getReclamacionDetDental(string Referencia, string Remesa, string Reclamacion, string MedicoID, DateTime FechaIni, DateTime FechaFin)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pReclamacion = "";
                string _pGrupo = "";
                string _pNomina = "";
                string _pBeneficiario = "";
                string _pPaciente = "";
                string _pFechaConsulta = "";
                string _pFechaPago = "";
                string _pEstatus = "";
                string _pOrgien = "";
                string _pCDT = "";
                string _pProcedimiento = "";
                string _pMonto = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sREFERENCIA", Referencia));
                lstParam.Add(sysConn.NewParm("@sNO_REMESA", Remesa));
                lstParam.Add(sysConn.NewParm("@sNO_CLRC", Reclamacion));
                lstParam.Add(sysConn.NewParm("@sPRPR_ID", MedicoID));
                lstParam.Add(sysConn.NewParm("@dFECHA_INI", FechaIni));
                lstParam.Add(sysConn.NewParm("@dFECHA_FIN", FechaFin));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.sp_aud_aclara_pagos_dental", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        _pReclamacion += "|" + dtResult.Rows[i][0].ToString();
                        _pGrupo += "|" + dtResult.Rows[i][1].ToString();
                        _pNomina += "|" + dtResult.Rows[i][2].ToString();
                        _pBeneficiario += "|" + dtResult.Rows[i][3].ToString();
                        _pPaciente += "|" + dtResult.Rows[i][4].ToString();
                        _pFechaConsulta += "|" + getFormatoFecha(dtResult.Rows[i][5].ToString(), true); // dtResult.Rows[i][5].ToString();
                        _pFechaPago += "|" + getFormatoFecha(dtResult.Rows[i][6].ToString(), true); // dtResult.Rows[i][6].ToString();
                        _pEstatus += "|" + dtResult.Rows[i][7].ToString();
                        _pOrgien += "|" + dtResult.Rows[i][8].ToString();
                        _pCDT += "|" + dtResult.Rows[i][9].ToString();
                        _pProcedimiento += "|" + dtResult.Rows[i][10].ToString();
                        _pMonto += "|" + dtResult.Rows[i][11].ToString();
                    }
                }

                retValue = "<dental>" +
                    "<reclamacion>" + _pReclamacion + "</reclamacion>" +
                    "<grupo>" + _pGrupo + "</grupo>" +
                    "<nomina>" + _pNomina + "</nomina>" +
                    "<beneficiario>" + _pBeneficiario + "</beneficiario>" +
                    "<paciente>" + _pPaciente + "</paciente>" +
                    "<fechaConsulta>" + _pFechaConsulta + "</fechaConsulta>" +
                    "<fechaPago>" + _pFechaPago + "</fechaPago>" +
                    "<estatus>" + _pEstatus + "</estatus>" +
                    "<origen>" + _pOrgien + "</origen>" +
                    "<cdt>" + _pCDT + "</cdt>" +
                    "<procedimiento>" + _pProcedimiento + "</procedimiento>" +
                    "<monto>" + _pMonto + "</monto>" +
                    "<errorcode>" + lstParam[6].Value.ToString() + "</errorcode>" +
                    "</dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><desc>" + sybex.ToString() + "</desc></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><desc>" + ex.ToString() + "</desc></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Servicios Referidos
        //*****************************************************************
        //          Servicios Referidos
        //*****************************************************************

        [WebMethod] // #26
        public string getServRefLista(int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pFechaCaptura = "";
                string _pCPT = "";
                string _pProcedimiento = "";
                string _pEspecialidadID = "";
                string _pEspecialidad = "";
                string _pFechaServicio = "";
                string _pProveedor = "";
                
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@pi_meme", CK));           
                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));

                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_OBTIENE_REFERIDO", lstParam);
                retValue = "<servref>";

                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        _pFechaCaptura = getFormatoFecha(dtResult.Rows[i][0].ToString(), false); // dtResult.Rows[i][0].ToString();
                        _pCPT = dtResult.Rows[i][1].ToString();
                        _pProcedimiento = dtResult.Rows[i][2].ToString();
                        _pEspecialidadID = dtResult.Rows[i][3].ToString();
                        _pEspecialidad = dtResult.Rows[i][4].ToString();
                        _pFechaServicio = getFormatoFecha(dtResult.Rows[i][5].ToString(), false); //dtResult.Rows[i][5].ToString();
                        _pProveedor = dtResult.Rows[i][6].ToString();

                        retValue += "<servicios>" +
                            "<fechaCaptura>" + _pFechaCaptura + "</fechaCaptura>" +
                            "<cpt>" + _pCPT + "</cpt>" +
                            "<procedimiento>" + _pProcedimiento + "</procedimiento>" +
                            "<especialidadID>" + _pEspecialidadID + "</especialidadID>" +
                            "<especialidad>" + _pEspecialidad + "</especialidad>" +
                            "<fechaServicio>" + _pFechaServicio + "</fechaServicio>" +
                            "<proveedor>" + _pProveedor + "</proveedor>" +
                        "</servicios>";
                    }   
                }
                else
                {
                    //retValue = "<servicios><error>-01</error><desc>sin datos</desc></servicios>";
                    retValue += "<servicios>" +
                            "<fechaCaptura>" + _pFechaCaptura + "</fechaCaptura>" +
                            "<cpt>" + _pCPT + "</cpt>" +
                            "<procedimiento>" + _pProcedimiento + "</procedimiento>" +
                            "<especialidadID>" + _pEspecialidadID + "</especialidadID>" +
                            "<especialidad>" + _pEspecialidad + "</especialidad>" +
                            "<fechaServicio>" + _pFechaServicio + "</fechaServicio>" +
                            "<proveedor>" + _pProveedor + "</proveedor>" +
                        "</servicios>";
                }

                retValue += "</servref>";
            }
            catch (AseException sybex)
            {
                retValue = "<servicios><error>-03</error><desc>" + sybex.ToString() + "</desc></servicios>";
            }
            catch (Exception ex)
            {
                retValue = "<servicios><error>-02</error><desc>" + ex.ToString() + "</desc></servicios>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #27
        public string getServRefLimite(int CK, DateTime Fecha, string CPT, string Preauto, int Entrada)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@pi_meme_ck", CK));
                lstParam.Add(sysConn.NewParm("@pd_f_serv", Fecha));
                lstParam.Add(sysConn.NewParm("@ps_ipcd_id", CPT));
                lstParam.Add(sysConn.NewParm("@ps_preau", Preauto));
                lstParam.Add(sysConn.NewParm("@ps_entrada", Entrada));

                lstParam.Add(sysConn.NewParmOut("@ps_cod_err", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPS_AUD_VALI_LIMITE", lstParam);
                retValue = "<servref><error>" + lstParam[5].Value.ToString() + "</error><desc></desc></servref>";
            }
            catch (AseException sybex)
            {
                retValue = "<servref><error>-03</error><desc>" + sybex.ToString() + "</desc></servref>";
            }
            catch (Exception ex)
            {
                retValue = "<servref><error>-02</error><desc>" + ex.ToString() + "</desc></servref>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #28
        public string getServRefValidaAutorizacion(string Preauto, string MedicoID, int CK, string CPT)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@sPREA", Preauto));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@sCPT", CPT));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_aud_val_autoriza_ref", lstParam);

                retValue = "<servref><error>" + lstParam[4].Value.ToString() + "</error><desc></desc></servref>";
            }
            catch (AseException sybex)
            {
                retValue = "<servref><error>-03</error><desc>" + sybex.ToString() + "</desc></servref>";
            }
            catch (Exception ex)
            {
                retValue = "<servref><error>-02</error><desc>" + ex.ToString() + "</desc></servref>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #29
        public string getServRefProveedorLab(int CK, DateTime Fecha, string CPT, string MedicoID)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pCPT = "";
                string _pProveedor = "";
                string _pProvDesc = "";

                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@dFSER", Fecha));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_OBTIENE_LABORATORIO", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _pCPT = dtResult.Rows[0][0].ToString();
                    _pProveedor = dtResult.Rows[0][1].ToString();
                    _pProvDesc = dtResult.Rows[0][2].ToString();
                }

                retValue = "<servref><servicios>" +
                    "<cpt>" + _pCPT + "</cpt>" +
                    "<proveedorid>" + _pProveedor + "</proveedorid>" +
                    "<Proveedordesc>" + _pProvDesc + "</Proveedordesc>" +
                    "</servicios></servref>";
            }
            catch (AseException sybex)
            {
                retValue = "<servref><error>-03</error><desc>" + sybex.ToString() + "</desc></servref>";
            }
            catch (Exception ex)
            {
                retValue = "<servref><error>-02</error><desc>" + ex.ToString() + "</desc></servref>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #30
        public string getServRefValidaProtesis(int CK, int Cliente, string Inca, string MedicoID, string CPT)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParm("@iGRGR", Cliente));
                lstParam.Add(sysConn.NewParm("@sINCAP", Inca));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));
                //lstParam.Add(sysConn.NewParm("", CPT));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPS_AUD_OBTIENE_CPO", lstParam);

                retValue = "<servref><error>" + lstParam[5].Value.ToString() + "</error><desc></desc></servref>";
            }
            catch (AseException sybex)
            {
                retValue = "<servref><error>-03</error><desc>" + sybex.ToString() + "</desc></servref>";
            }
            catch (Exception ex)
            {
                retValue = "<servref><error>-02</error><desc>" + ex.ToString() + "</desc></servref>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #31
        public string getServRefValidaAutorizacionPVH(string NumAutorizacion, string MedicoID, int CK)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sPREA", NumAutorizacion));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_aud_valida_autoriza", lstParam);

                retValue = "<autoriza><error>" + lstParam[3].Value.ToString() + "</error><desc></desc></autoriza>";
            }
            catch (AseException sybex)
            {
                retValue = "<autoriza><error>-03</error><desc>" + sybex.ToString() + "</desc></autoriza>";
            }
            catch (Exception ex)
            {
                retValue = "<autoriza><error>-02</error><desc>" + ex.ToString() + "</desc></autoriza>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Dental
        //***************************************************************** Dental

        [WebMethod] // #32
        public string den_audiomatico_02(string MedicoID, string Elegibilidad, string Preautorizacion, string FechaServicio)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sAUTO", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@sPREA", Preautorizacion));
                lstParam.Add(sysConn.NewParm("@dFSER", FechaServicio));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_aud_dental_02", lstParam);

                retValue = "<dental><errorcode>" + lstParam[4].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #33
        public string den_audiomatico_03(string MedicoID, string Elegibilidad, string ICD, string LugarServicio, string CDT, string DienteNum, string DienteIni, string DienteFin, string Superficie)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sAUTO", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@sIDCD", ICD));
                lstParam.Add(sysConn.NewParm("@sLSER", LugarServicio));
                lstParam.Add(sysConn.NewParm("@sIPCD", CDT));
                lstParam.Add(sysConn.NewParm("@sDENT", DienteNum));
                lstParam.Add(sysConn.NewParm("@sDENT_INI", DienteIni));
                lstParam.Add(sysConn.NewParm("@sDENT_FIN", DienteFin));
                lstParam.Add(sysConn.NewParm("@sSURF", Superficie));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                sysConn.sybaseExecuteQuery("dbo.sp_aud_dental_03", lstParam);

                retValue = "<dental><errorcode>" + lstParam[9].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";

            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #34
        public string den_audiomatico_04(string MedicoID, string Elegibilidad, string Indicador, string IncapacidadTipo, string IncapacidadDias, DateTime IncapacidadFecha, string Folio)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@sAUTO", MedicoID));
                lstParam.Add(sysConn.NewParm("@sHOST", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@iEXPA", Indicador));
                lstParam.Add(sysConn.NewParm("@sINCAPACIDAD", IncapacidadTipo));
                lstParam.Add(sysConn.NewParm("@sDIASINCA", IncapacidadDias));
                lstParam.Add(sysConn.NewParm("@dFECHAINCA", IncapacidadFecha));
                lstParam.Add(sysConn.NewParm("@sFOLIO", Folio));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                lstParam.Add(sysConn.NewParmOut("@sCLRC", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_aud_dental_04", lstParam);

                retValue = "<dental><errorcode>" + lstParam[7].Value.ToString() + "</errorcode><claveret>" + lstParam[8].Value.ToString() +"</claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
                

            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #35
        public string den_preautorizacion(int Preautorizacion, int Secuencia, string Elegibilidad, int CK, int Cliente, string Estatus, string FechaCaptura, string FechaAutoriza, string FechaServicio, string MedicoID,
            string ProveedorID, string CDT, string Diente, string Superficie, int Unidades, string MotivoRechazo)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@iautoriza", Preautorizacion));
                lstParam.Add(sysConn.NewParm("@isecuencial", Secuencia));
                lstParam.Add(sysConn.NewParm("@selegibilidad", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@imeme", CK));
                lstParam.Add(sysConn.NewParm("@igrgr", Cliente));
                lstParam.Add(sysConn.NewParm("@sestatus", Estatus));
                lstParam.Add(sysConn.NewParm("@fecha_captura", FechaCaptura));
                lstParam.Add(sysConn.NewParm("@fecha_autoriza", FechaAutoriza));
                lstParam.Add(sysConn.NewParm("@fecha_servicio", FechaServicio));
                lstParam.Add(sysConn.NewParm("@sprpr_consulta", MedicoID));
                lstParam.Add(sysConn.NewParm("@sprpr_autoriza", ProveedorID));
                lstParam.Add(sysConn.NewParm("@sprocedimiento", CDT));
                lstParam.Add(sysConn.NewParm("@sno_diente", Diente));
                lstParam.Add(sysConn.NewParm("@ssuperficie", Superficie));
                lstParam.Add(sysConn.NewParm("@iunidades", Unidades));
                lstParam.Add(sysConn.NewParm("@smotivo", MotivoRechazo));

                lstParam.Add(sysConn.NewParmOut("@ps_cod_err", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_PREAUT_DENTAL", lstParam);
                retValue = "<dental><errorcode>" + lstParam[16].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #36
        public string den_plan_dental_validar(int CK, DateTime FechaServicio, string CDT, string MedicoID, string DienteNum, string DienteIni, string DienteFin, string Superficie)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@pi_meme_ck", CK));
                lstParam.Add(sysConn.NewParm("@pd_f_serv", FechaServicio));
                lstParam.Add(sysConn.NewParm("@ps_ipcd_id", CDT));
                lstParam.Add(sysConn.NewParm("@ps_prpr_id", MedicoID));
                lstParam.Add(sysConn.NewParm("@ps_diente", DienteNum));
                lstParam.Add(sysConn.NewParm("@ps_diente_ini", DienteIni));
                lstParam.Add(sysConn.NewParm("@ps_diente_fin", DienteFin));
                lstParam.Add(sysConn.NewParm("@ps_superficie", Superficie));
                lstParam.Add(sysConn.NewParmOut("@ps_sese_id", DbType.String));
                lstParam.Add(sysConn.NewParmOut("@ps_cod_err", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.sp_aud_sel_serv_dental", lstParam);
                retValue = "<dental><errorcode>" + lstParam[9].Value.ToString() + "</errorcode><claveret>" + lstParam[8].Value.ToString() + "</claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #37
        public string den_plan_dental_guardar(string MedicoID, string ElegibilidadActual, string ElegibilidadInicial, int CK, string ICD, string LugarServicio, string CPT, string DienteNum, string DienteIni, string DienteFin, string Superficie)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sAUTO", ElegibilidadActual));
                lstParam.Add(sysConn.NewParm("@IMEME", CK));
                lstParam.Add(sysConn.NewParm("@sIDCD", ICD));
                lstParam.Add(sysConn.NewParm("@sLSER", LugarServicio));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));
                lstParam.Add(sysConn.NewParm("@sDENT", DienteNum));
                lstParam.Add(sysConn.NewParm("@sDENT_INI", DienteIni));
                lstParam.Add(sysConn.NewParm("@sDENT_FIN", DienteFin));
                lstParam.Add(sysConn.NewParm("@sSURF", Superficie));
                lstParam.Add(sysConn.NewParm("@sELEG", ElegibilidadInicial)); //230208
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_PLAN_DENTAL", lstParam);
                retValue = "<dental><errorcode>" + lstParam[11].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #38
        public string den_estado_inicial_guardar(string MedicoID, string Elegibilidad, int CK, string ICD, string CPT, string DienteNum, string DienteIni, string DienteFin, string Superficie, string Nivel)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@sAUTO", Elegibilidad));
                lstParam.Add(sysConn.NewParm("@IMEME", CK));
                lstParam.Add(sysConn.NewParm("@sIDCD", ICD));
                lstParam.Add(sysConn.NewParm("@sIPCD", CPT));
                lstParam.Add(sysConn.NewParm("@sDENT", DienteNum));
                lstParam.Add(sysConn.NewParm("@sDENT_INI", DienteIni));
                lstParam.Add(sysConn.NewParm("@sDENT_FIN", DienteFin));
                lstParam.Add(sysConn.NewParm("@sSURF", Superficie));
                lstParam.Add(sysConn.NewParm("@sNIVEL", Nivel));

                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_DIAG_PRESUNCION", lstParam);

                retValue = "<dental><errorcode>" + lstParam[10].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #39
        public string den_hc_aparatos_sistemas(string elegibilidad, int CK, string MedicoID, string Disfagia,
                               string Vomito, string Diarrea, string Pirosis, string Otros1, string Rinorrea, string Disnea, string Cianosis,
                               string Otros2, string Taquicardia, string Bradicardia, string Lipotimia, string DolorPrecordial, string Otros3,
                               string Disuria, string Edema, string Poliuria, string Hematuria, string Otros4, string DiferenciaPeso, string CambioFrioCalor,
                               string Exoftalmos, string Otros5, string Hemorragia, string Epistaxis, string Petequias, string Adenopatias, string Otros6,
                               string Convulsiones, string Parestesia, string Vertigo, string Temblor, string Otros7, string DeformidadArticular, string DolorArticular,
                               string LimiteMovimiento, string Otros8)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@NO_AUTORIZACION", elegibilidad));
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParm("@PRPR_ID", MedicoID));
                lstParam.Add(sysConn.NewParm("@ADIGDISFRAGIA", Disfagia));
                lstParam.Add(sysConn.NewParm("@ADIGVOMITO", Vomito));
                lstParam.Add(sysConn.NewParm("@ADIGDIARREA", Diarrea));
                lstParam.Add(sysConn.NewParm("@ADIGPIROSIS", Pirosis));
                lstParam.Add(sysConn.NewParm("@ADIGOTROS", Otros1));
                lstParam.Add(sysConn.NewParm("@ARESPRINORREA", Rinorrea));
                lstParam.Add(sysConn.NewParm("@ARESPDISNEA", Disnea));
                lstParam.Add(sysConn.NewParm("@ARESPCIANOSIS", Cianosis));
                lstParam.Add(sysConn.NewParm("@ARESPOTROS", Otros2));
                lstParam.Add(sysConn.NewParm("@ACARDIOTAQUICARDIA", Taquicardia));
                lstParam.Add(sysConn.NewParm("@ACARDIOBRADICARDIA", Bradicardia));
                lstParam.Add(sysConn.NewParm("@ACARDIOLIPOTIMIA", Lipotimia));
                lstParam.Add(sysConn.NewParm("@ACARDIOPRECORDIAL", DolorPrecordial));
                lstParam.Add(sysConn.NewParm("@ACARDIOOTROS", Otros3));
                lstParam.Add(sysConn.NewParm("@AGENIDISURIA", Disuria));
                lstParam.Add(sysConn.NewParm("@AGENIEDEMA", Edema));
                lstParam.Add(sysConn.NewParm("@AGENIPOLIURIA", Poliuria));
                lstParam.Add(sysConn.NewParm("@AGENIHEMATURIA", Hematuria));
                lstParam.Add(sysConn.NewParm("@AGENIOTROS", Otros4));
                lstParam.Add(sysConn.NewParm("@SENDOPESO", DiferenciaPeso));
                lstParam.Add(sysConn.NewParm("@SENDOINTOLERANCIA", CambioFrioCalor));
                lstParam.Add(sysConn.NewParm("@SENDOEXOFTALMOS", Exoftalmos));
                lstParam.Add(sysConn.NewParm("@SENDOOTROS", Otros5));
                lstParam.Add(sysConn.NewParm("@SHEMOHEMORRAGIA", Hemorragia));
                lstParam.Add(sysConn.NewParm("@SHEMOEPISTAXIS", Epistaxis));
                lstParam.Add(sysConn.NewParm("@SHEMOPETEQUIAS", Petequias));
                lstParam.Add(sysConn.NewParm("@SHEMOADENOPATIAS", Adenopatias));
                lstParam.Add(sysConn.NewParm("@SHEMOOTROS", Otros6));
                lstParam.Add(sysConn.NewParm("@SNERVCONVULSIONES", Convulsiones));
                lstParam.Add(sysConn.NewParm("@SNERVPARESTESIA", Parestesia));
                lstParam.Add(sysConn.NewParm("@SNERVVERTIGO", Vertigo));
                lstParam.Add(sysConn.NewParm("@SNERVTEMBLOR", Temblor));
                lstParam.Add(sysConn.NewParm("@SNERVOTROS", Otros7));
                lstParam.Add(sysConn.NewParm("@SESQUEDEFORMIAD", DeformidadArticular));
                lstParam.Add(sysConn.NewParm("@SESQUEDOLOR", DolorArticular));
                lstParam.Add(sysConn.NewParm("@SESQUELIMITACION", LimiteMovimiento));
                lstParam.Add(sysConn.NewParm("@SESQUEOTROS", Otros8));

                lstParam.Add(sysConn.NewParmOut("@error", DbType.String));
                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_DEN_HCAPA_SIS", lstParam);

                retValue = "<dental><errorcode>" + lstParam[40].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #40
        public string den_hc_evaluacion(string elegibilidad, int CK, string MedicoID, string Cara, string Cuello, string Articulacion, string Ganglios,
            string Labios, string Carrillo, string Encias, string Lengua, string Salivales, string Paladar, string Amigdalas, string PisoBoca, string Ulceras1,
            string Ulceras2, string Ulceras3, string Chasquido, string DolorPalpacion, string AperturaReducida, string DolorApertura, string DificultadApertura,
            string Succion, string Deglucion, string Respiracion, string Onicofagia, string Burxismo, string Otros, string Oclusion, string Cepilla, string Tecnica, string MarcaCepillo,
            string PastaCepillo, string Enjuague, string Hilo)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@NO_AUTORIZACION", elegibilidad));
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParm("@PRPR_ID", MedicoID));
                lstParam.Add(sysConn.NewParm("@EFACCARA", Cara));
                lstParam.Add(sysConn.NewParm("@EFACCUELLO", Cuello));
                lstParam.Add(sysConn.NewParm("@EFACARTICULACION", Articulacion));
                lstParam.Add(sysConn.NewParm("@EFACGANGLIOS", Ganglios));
                lstParam.Add(sysConn.NewParm("@EFACLABIOS", Labios));
                lstParam.Add(sysConn.NewParm("@EFACCARRILLO", Carrillo));
                lstParam.Add(sysConn.NewParm("@EFACENCIAS", Encias));
                lstParam.Add(sysConn.NewParm("@EFACLENGUA", Lengua));
                lstParam.Add(sysConn.NewParm("@EFACSALIVALES", Salivales));
                lstParam.Add(sysConn.NewParm("@EFACPALADAR", Paladar));
                lstParam.Add(sysConn.NewParm("@EFACAMIGDALAS", Amigdalas));
                lstParam.Add(sysConn.NewParm("@EFACPISO", PisoBoca));
                lstParam.Add(sysConn.NewParm("@EFACULCERAS1", Ulceras1));
                lstParam.Add(sysConn.NewParm("@EFACULCERAS2", Ulceras2));
                lstParam.Add(sysConn.NewParm("@EFACULCERAS3", Ulceras3));
                lstParam.Add(sysConn.NewParm("@ETEMCHASQUIDO", Chasquido));
                lstParam.Add(sysConn.NewParm("@ETEMPALPACION", DolorPalpacion));
                lstParam.Add(sysConn.NewParm("@ETEMAPERTURA", AperturaReducida));
                lstParam.Add(sysConn.NewParm("@ETEMCIERRE", DolorApertura));
                lstParam.Add(sysConn.NewParm("@ETEMDIFICULTAD", DificultadApertura));
                lstParam.Add(sysConn.NewParm("@EHABSUCCION", Succion));
                lstParam.Add(sysConn.NewParm("@EHABDEGLUCION", Deglucion));
                lstParam.Add(sysConn.NewParm("@EHABRESPIRACION", Respiracion));
                lstParam.Add(sysConn.NewParm("@EHABONICOFAGIA", Onicofagia));
                lstParam.Add(sysConn.NewParm("@EHABBRUXISMO", Burxismo));
                lstParam.Add(sysConn.NewParm("@EHABOTROS", Otros));
                lstParam.Add(sysConn.NewParm("@EHABOCLUSION", Oclusion));
                lstParam.Add(sysConn.NewParm("@EHIGCEPILLA", Cepilla));
                lstParam.Add(sysConn.NewParm("@EHIGTECNICAS", Tecnica));
                lstParam.Add(sysConn.NewParm("@EHIGCEPILLO", MarcaCepillo));
                lstParam.Add(sysConn.NewParm("@EHIGPASTA", PastaCepillo));
                lstParam.Add(sysConn.NewParm("@EHIGENJUAGUE", Enjuague));
                lstParam.Add(sysConn.NewParm("@EHIGHILO", Hilo));
                lstParam.Add(sysConn.NewParmOut("@error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_DEN_HCEVAL", lstParam);
                retValue = "<dental><errorcode>" + lstParam[36].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #41
        public string den_hc_ap_NoPatologicos(string elegibilidad, int CK, string MedicoID, string Antibioticos, string Sulfas, string Yodo, string Animales, string Anestesicos, string Aspirinas, string Barbituricos, string Sedantes,
            string Alimentos, string Otros, string Especificar, string Anesteciado, string Problemas, string Fumar, string Beber, string Sustancias, string Enfermedad, string EnfermedadDesc, string Medicamento, string MedicamentoDesc,
            string Embarazo, string SemanasEmbarazo, string Amamantando, string Presion, string UltimaConsulta, string MotivoConsulta, string OrigenEtnico)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@NO_AUTORIZACION", elegibilidad));
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParm("@PRPR_ID", MedicoID));
                lstParam.Add(sysConn.NewParm("@ALERANTIBIOTICO", Antibioticos));
                lstParam.Add(sysConn.NewParm("@ALERSULFAS", Sulfas));
                lstParam.Add(sysConn.NewParm("@ALERYODO", Yodo));
                lstParam.Add(sysConn.NewParm("@ALERANIMALES", Animales));
                lstParam.Add(sysConn.NewParm("@ALERANESTESICOS", Anestesicos));
                lstParam.Add(sysConn.NewParm("@ALERASPIRINAS", Aspirinas));
                lstParam.Add(sysConn.NewParm("@ALERBARBIURICOS", Barbituricos));
                lstParam.Add(sysConn.NewParm("@ALERSEDANTES", Sedantes));
                lstParam.Add(sysConn.NewParm("@ALERALIMENTOS", Alimentos));
                lstParam.Add(sysConn.NewParm("@ALEROTRO", Otros));
                lstParam.Add(sysConn.NewParm("@ALERDESCRIPCION", Especificar));
                lstParam.Add(sysConn.NewParm("@ANESANTERIOR", Anesteciado));
                lstParam.Add(sysConn.NewParm("@PROBLEMAPLICACION", Problemas));
                lstParam.Add(sysConn.NewParm("@FUMA", Fumar));
                lstParam.Add(sysConn.NewParm("@ALCOHOL", Beber));
                lstParam.Add(sysConn.NewParm("@SUSTANCIAS", Sustancias));
                lstParam.Add(sysConn.NewParm("@PADECERENFERMEDAD", Enfermedad));
                lstParam.Add(sysConn.NewParm("@DESCENFERMEDAD", EnfermedadDesc));
                lstParam.Add(sysConn.NewParm("@TOMADOMEDICAMENTO", Medicamento));
                lstParam.Add(sysConn.NewParm("@CUALMEDICAMENTO", MedicamentoDesc));
                lstParam.Add(sysConn.NewParm("@EMBARAZADA", Embarazo));
                lstParam.Add(sysConn.NewParm("@SEMANAS", SemanasEmbarazo));
                lstParam.Add(sysConn.NewParm("@AMAMANTANDO", Amamantando));
                lstParam.Add(sysConn.NewParm("@PRESIONARTERIAL", Presion));
                lstParam.Add(sysConn.NewParm("@ULTIMACONSULTADEN", UltimaConsulta));
                lstParam.Add(sysConn.NewParm("@ENFERMEDADRIESGO", MotivoConsulta));
                lstParam.Add(sysConn.NewParm("@ORGIEN", OrigenEtnico));

                lstParam.Add(sysConn.NewParmOut("@error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_DEN_HCNOPAT", lstParam);
                retValue = "<dental><errorcode>" + lstParam[30].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #42
        public string den_hc_ap_patologicos(string elegibilidad, int CK, string MedicoID, string diabetesPadres, string diabetesHermanos, string diabetesHijos, string diabetesPareja, string diabetesTios, string diabetesAbuelos,
            string cancerPadres, string cancerHermanos, string cancerHijos, string cancerPareja, string cancerTios, string cancerAbuelos, string Enfermedad, string Hospitalizado, string Cardiopatia, string Fiebre, string Artritis, string Tuberculosis,
            string Psiquiatricos, string Neoplasicas, string ETS, string Congenitas, string Hemorragias, string Apoplejia, string Marcapasos, string Epilepcia, string Anemia, string Diabetes, string Hipertension)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();

                lstParam.Add(sysConn.NewParm("@NO_AUTORIZACION", elegibilidad));
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParm("@PRPR_ID", MedicoID));
                lstParam.Add(sysConn.NewParm("@DIABEPADRE", diabetesPadres));
                lstParam.Add(sysConn.NewParm("@DIABEHERMANO", diabetesHermanos));
                lstParam.Add(sysConn.NewParm("@DIABEHIJO", diabetesHijos));
                lstParam.Add(sysConn.NewParm("@DIABEESPOSO", diabetesPareja));
                lstParam.Add(sysConn.NewParm("@DIABETIOS", diabetesTios));
                lstParam.Add(sysConn.NewParm("@DIABEABUELOS", diabetesAbuelos));
                lstParam.Add(sysConn.NewParm("@CANCERPADRE", cancerPadres));
                lstParam.Add(sysConn.NewParm("@CANCERHERMANO", cancerHermanos));
                lstParam.Add(sysConn.NewParm("@CANCERHIJO", cancerHijos));
                lstParam.Add(sysConn.NewParm("@CANCERESPOSO", cancerPareja));
                lstParam.Add(sysConn.NewParm("@CANCERTIOS", cancerTios));
                lstParam.Add(sysConn.NewParm("@CANCERABUELOS", cancerAbuelos));
                lstParam.Add(sysConn.NewParm("@ENFULTIMOANIO", Enfermedad));
                lstParam.Add(sysConn.NewParm("@HOSPULTIMODOS", Hospitalizado));
                lstParam.Add(sysConn.NewParm("@CARDIOPATIA", Cardiopatia));
                lstParam.Add(sysConn.NewParm("@FIEBREREUMA", Fiebre));
                lstParam.Add(sysConn.NewParm("@ARTRITIS", Artritis));
                lstParam.Add(sysConn.NewParm("@TUBERCULOSIS", Tuberculosis));
                lstParam.Add(sysConn.NewParm("@TRATPSIQUIATRICO", Psiquiatricos));
                lstParam.Add(sysConn.NewParm("@ENFNEOPLASICA", Neoplasicas));
                lstParam.Add(sysConn.NewParm("@ENFTRANSMITESEXUAL", ETS));
                lstParam.Add(sysConn.NewParm("@ENFCONGENITA", Congenitas));
                lstParam.Add(sysConn.NewParm("@HEMORRAGIAS", Hemorragias));
                lstParam.Add(sysConn.NewParm("@APOPLEJIA", Apoplejia));
                lstParam.Add(sysConn.NewParm("@MARCAPASOS", Marcapasos));
                lstParam.Add(sysConn.NewParm("@EPILEPSIA", Epilepcia));
                lstParam.Add(sysConn.NewParm("@ANEMIA", Anemia));
                lstParam.Add(sysConn.NewParm("@DIABETES", Diabetes));
                lstParam.Add(sysConn.NewParm("@HIPERTENSION", Hipertension));
                lstParam.Add(sysConn.NewParmOut("@error", DbType.String));

                sysConn.sybaseExecuteQuery("dbo.SPI_AUD_DEN_HCPAT", lstParam);
                retValue = "<dental><errorcode>" + lstParam[10].Value.ToString() + "</errorcode><claveret></claveret></dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<dental><errorcode>-03</errorcode><claveret>" + sybex.ToString() + "</claveret></dental>";
            }
            catch (Exception ex)
            {
                retValue = "<dental><errorcode>-02</errorcode><claveret>" + ex.ToString() + "</claveret></dental>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #43
        public string den_get_historial_dental(int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pReclamacion = "";
                string _pConsecutivo = "";
                string _pCKcode = "";
                string _pMedicoID = "";
                string _pCDT = "";
                string _pFechaServicio = "";
                string _pDienteNum = "";
                string _pDienteIni = "";
                string _pDienteFin = "";
                string _pSuperficie = "";
                string _pUnidades = "";
                string _pCodigoDiente = "";
                string _pCodigoUtilizacion = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParmOut("@ps_error", DbType.String));
                dtResult = sysConn.sybaseGetTable("dbo.SPS_AUD_DENTAL_HIST", lstParam);

                //  <dental>
                //      <reclamacion>|020302717100|020302717100|020302717100</reclamacion>
                //      <consecutivo>|1|2|3</consecutivo>
                //      <ckcode>|1003201|1003201|1003201</ckcode>
                //      <medicoID>|01030558|01030558|01030558</medicoID>
                //      <CDT>|00120|00120|00120</CDT>
                //      <fechaServicio>|2002-01-02 00:00:00.0|2002-01-02 00:00:00.0|2002-01-02 00:00:00.0</fechaServicio>
                //      <numDiente>|13|19|19</numDiente>
                //      <dInicial>|13|19|19|</dInicial>
                //      <dFinal>|13|19|19|</dFinal>
                //      <superficie>| | </superficie>
                //      <unidades>|1|1|1</unidades>
                //      <codigoDiente>| | | </codigoDiente>
                //      <codigoUtilizacion>|FIL|FIL|XRS</codigoUtilizacion>
                //      <errorcode>00</errorcode>
                //  </dental>

                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        _pReclamacion += "|" + dtResult.Rows[i][0].ToString();
                        _pConsecutivo += "|" + dtResult.Rows[i][1].ToString();
                        _pCKcode += "|" + dtResult.Rows[i][2].ToString();
                        _pMedicoID += "|" + dtResult.Rows[i][3].ToString();
                        _pCDT += "|" + dtResult.Rows[i][4].ToString();
                        _pFechaServicio += "|" + getFormatoFecha(dtResult.Rows[i][5].ToString(), false); //dtResult.Rows[i][5].ToString();
                        _pDienteNum += "|" + dtResult.Rows[i][6].ToString();
                        _pDienteIni += "|" + dtResult.Rows[i][7].ToString();
                        _pDienteFin += "|" + dtResult.Rows[i][8].ToString();
                        _pSuperficie += "|" + dtResult.Rows[i][9].ToString();
                        _pUnidades += "|" + dtResult.Rows[i][10].ToString();
                        _pCodigoDiente += "|" + dtResult.Rows[i][11].ToString();
                        _pCodigoUtilizacion += "|" + dtResult.Rows[i][12].ToString();
                    }
                }

                retValue = "<dental>" +
                    "<reclamacion>" + _pReclamacion + "</reclamacion>" +
                    "<consecutivo>" + _pConsecutivo + "</consecutivo>" +
                    "<ckcode>" + _pCKcode + "</ckcode>" +
                    "<medicoID>" + _pMedicoID + "</medicoID>" +
                    "<CDT>" + _pCDT + "</CDT>" +
                    "<fechaServicio>" + _pFechaServicio + "</fechaServicio>" +
                    "<numDiente>" + _pDienteNum + "</numDiente>" +
                    "<dInicial>" + _pDienteIni + "</dInicial>" +
                    "<dFinal>" + _pDienteFin + "</dFinal>" +
                    "<superficie>" + _pSuperficie + "</superficie>" +
                    "<unidades>" + _pUnidades + "</unidades>" +
                    "<codigoDiente>" + _pCodigoDiente + "</codigoDiente>" +
                    "<codigoUtilizacion>" + _pCodigoUtilizacion + "</codigoUtilizacion>" +
                    "<errorcode>" + lstParam[1].Value.ToString() + "</errorcode>" +
                    "</dental>";
            }
            catch (AseException sybex)
            {
                retValue = "<servref><error>-03</error><desc>" + sybex.ToString() + "</desc></servref>";
            }
            catch (Exception ex)
            {
                retValue = "<servref><error>-02</error><desc>" + ex.ToString() + "</desc></servref>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Rehabilitacion
        [WebMethod] // #45
        public string getSesionesRehab(int CK)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@iMEME", CK));                    // [0]
                lstParam.Add(sysConn.NewParmOut("@iSESION", DbType.Int32));     // [1]
                lstParam.Add(sysConn.NewParmOut("@dFECHA", DbType.DateTime));   // [2]
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));       // [3]

                sysConn.sybaseExecuteQuery("dbo.SPS_AUD_VALIDA_PREAUTORIZA", lstParam);

                string fecha = getFormatoFecha(lstParam[2].Value.ToString(), true);
                //<rehab><error>00</error><desc></desc><sesiones>0</sesiones><fecha>2000-01-01</fecha></rehab>
                retValue = "<rehab>" +
                    "<error>" + lstParam[3].Value.ToString() + "</error>" +
                    "<desc></desc>" +
                    "<sesiones>" + lstParam[1].Value.ToString() + "</sesiones>" +
                    "<fecha>" + fecha + "</fecha>" +
                "</rehab>";
            }
            catch (AseException sybex)
            {
                retValue = "<rehab><error>-03</error><desc>" + sybex.ToString() + "</desc></rehab>";
            }
            catch (Exception ex)
            {
                retValue = "<rehab><error>-02</error><desc>" + ex.ToString() + "</desc></rehab>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #46
        public string getMedicoAvanza(string MedicoID)
        {
            string retValue = "";
            SybaseData sysConn = new SybaseData();

            try
            {
                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@VCH_PRPR_ID", MedicoID));            // [0]
                lstParam.Add(sysConn.NewParmOut("@VCH_TIPO_PROV", DbType.String));  // [1]

                sysConn.sybaseExecuteQuery("dbo.SP_AUD_PRPR_MEDICINA_FISICA", lstParam);

                retValue = "<rehab><error>00</error><desc>"+ lstParam[1].Value.ToString() + "</desc></rehab>";
            }
            catch (AseException sybex)
            {
                retValue = "<rehab><error>-03</error><desc>" + sybex.ToString() + "</desc></rehab>";
            }
            catch (Exception ex)
            {
                retValue = "<rehab><error>-02</error><desc>" + ex.ToString() + "</desc></rehab>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion

        #region Cirugia Programada
        [WebMethod] // #54
        public string CirugiaGetProcedimientos(int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pVitaError = "";
                string _cpt = "";
                string _procedimiento = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                dtResult = sysConn.sybaseGetTable("dbo.SP_AUD_IPCD_HOSP_CE", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _pVitaError = lstParam[1].Value.ToString(); //_pVitaError = dtResult.Rows[0][0].ToString();
                    retValue = "<cirugia><error>" + _pVitaError + "</error>";

                    if (dtResult.Columns.Count > 1)
                    {
                        for (int i = 0; i < dtResult.Rows.Count; i++)
                        {
                            _cpt += "|" + dtResult.Rows[i][0].ToString();
                            _procedimiento += "|" + dtResult.Rows[i][1].ToString();
                        }
                    }

                    retValue +=
                        "<cpt>" + _cpt + "</cpt>" +
                        "<proc>" + _procedimiento + "</proc>" +
                    "</cirugia>";
                }
                else
                {
                    retValue = "<cirugia><error>-01</error><desc>Tabla sin registros</desc></cirugia>";
                }
            }
            catch (AseException sybex)
            {
                retValue = "<cirugia><error>-03</error><desc>" + sybex.ToString() + "</desc></cirugia>";
            }
            catch (Exception ex)
            {
                retValue = "<cirugia><error>-02</error><desc>" + ex.ToString() + "</desc></cirugia>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #55
        public string CirugiaGetHospitales(int CK, string Estado)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pVitaError = "";
                string _id = "";
                string _hospital = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@MEME_CK", CK));
                lstParam.Add(sysConn.NewParm("@EDO_ID", Estado));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                dtResult = sysConn.sybaseGetTable("dbo.SP_AUD_REVE_HOSP_CE", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _pVitaError = lstParam[2].Value.ToString(); //_pVitaError = dtResult.Rows[0][0].ToString();
                    retValue = "<cirugia><error>" + _pVitaError + "</error>";

                    if (dtResult.Columns.Count > 1)
                    {
                        for (int i = 0; i < dtResult.Rows.Count; i++)
                        {
                            _id += "|" + dtResult.Rows[i][0].ToString();
                            _hospital += "|" + dtResult.Rows[i][1].ToString();
                        }
                    }

                    retValue +=
                        "<id>" + _id + "</id>" +
                        "<hosp>" + _hospital + "</hosp>" +
                    "</cirugia>";
                }
                else
                {
                    retValue = "<cirugia><error>-01</error><desc>Tabla sin registros</desc></cirugia>";
                }
            }
            catch (AseException sybex)
            {
                retValue = "<cirugia><error>-03</error><desc>" + sybex.ToString() + "</desc></cirugia>";
            }
            catch (Exception ex)
            {
                retValue = "<cirugia><error>-02</error><desc>" + ex.ToString() + "</desc></cirugia>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }

        [WebMethod] // #56
        public string CirugiaGetDetalle(string PA, string MedicoID, int CK)
        {
            string retValue = "";
            DataTable dtResult = new DataTable();
            SybaseData sysConn = new SybaseData();

            try
            {
                string _pVitaError = "";
                string _pa = "";
                string _estatus = "";
                string _cpt = "";
                string _proc = "";
                string _monto = "";
                string _unidades = "";
                string _apoyo1 = "";
                string _apoyo2 = "";
                string _notas = "";

                List<AseParameter> lstParam = new List<AseParameter>();
                lstParam.Add(sysConn.NewParm("@sPREA", PA));
                lstParam.Add(sysConn.NewParm("@sPRPR", MedicoID));
                lstParam.Add(sysConn.NewParm("@iMEME", CK));
                lstParam.Add(sysConn.NewParmOut("@sret", DbType.String));
                dtResult = sysConn.sybaseGetTable("dbo.SP_AUD_PREA_HOSP_CE", lstParam);

                if (dtResult.Rows.Count > 0)
                {
                    _pVitaError = lstParam[3].Value.ToString();
                    retValue = "<cirugia><error>" + _pVitaError + "</error>";

                    if (dtResult.Columns.Count > 1)
                    {
                        for (int i = 0; i < dtResult.Rows.Count; i++)
                        {
                            _pa += "|" + dtResult.Rows[i][0].ToString();
                            _estatus += "|" + dtResult.Rows[i][1].ToString();
                            _cpt += "|" + dtResult.Rows[i][2].ToString();
                            _proc += "|" + dtResult.Rows[i][3].ToString();
                            _monto += "|" + dtResult.Rows[i][4].ToString();
                            _unidades += "|" + dtResult.Rows[i][5].ToString();
                            _apoyo1 += "|" + dtResult.Rows[i][6].ToString();
                            _apoyo2 += "|" + dtResult.Rows[i][7].ToString();
                            _notas += "|" + dtResult.Rows[i][8].ToString();
                        }
                    }

                    retValue +=
                        "<pa>" + _pa + "</pa>" +
                        "<estatus>" + _estatus + "</estatus>" +
                        "<cpt>" + _cpt + "</cpt>" +
                        "<proc>" + _proc + "</proc>" +
                        "<monto>" + _monto + "</monto>" +
                        "<unidades>" + _unidades + "</unidades>" +
                        "<apoyo1>" + _apoyo1 + "</apoyo1>" +
                        "<apoyo2>" + _apoyo2 + "</apoyo2>" +
                        "<notas>" + _notas + "</notas>" +
                    "</cirugia>";
                }
                else
                {
                    retValue = "<cirugia><error>-01</error><desc>Tabla sin registros</desc></cirugia>";
                }
            }
            catch (AseException sybex)
            {
                retValue = "<cirugia><error>-03</error><desc>" + sybex.ToString() + "</desc></cirugia>";
            }
            catch (Exception ex)
            {
                retValue = "<cirugia><error>-02</error><desc>" + ex.ToString() + "</desc></cirugia>";
            }
            finally
            {
                sysConn.closeConnection();
            }

            return retValue;
        }
        #endregion
    }
}
