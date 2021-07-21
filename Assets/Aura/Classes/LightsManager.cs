/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Although Aura (or Aura 1) is still a free project, it is not    *
*          open-source nor in the public domain anymore.                   *
*          Aura is now governed by the End Used License Agreement of       *
*          the Asset Store of Unity Technologies.                          *
*                                                                          * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/ 

using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Manages the lights, collects and packs data, shadow maps and cookie maps
    /// </summary>
    public class LightsManager
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public LightsManager()
        {
            DirectionalLightsManager = new DirectionalLightsManager();
            SpotLightsManager = new SpotLightsManager();
            PointLightsManager = new PointLightsManager();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the directional AuraLights manager
        /// </summary>
        public DirectionalLightsManager DirectionalLightsManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Accessor to the spot AuraLights manager
        /// </summary>
        public SpotLightsManager SpotLightsManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Accessor to the point AuraLights manager
        /// </summary>
        public PointLightsManager PointLightsManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Disposes the different managers
        /// </summary>
        public void Dispose()
        {
            DirectionalLightsManager.Dispose();
            SpotLightsManager.Dispose();
            PointLightsManager.Dispose();
        }

        /// <summary>
        /// Updates the different managers
        /// </summary>
        public void Update()
        {
            DirectionalLightsManager.Update();
            SpotLightsManager.Update();
            PointLightsManager.Update();
        }

        /// <summary>
        /// Registers an AuraLight onto the correct manager
        /// </summary>
        /// <param name="light">The candidate light</param>
        public void Register(AuraLight light, bool castShadows, bool castCookie)
        {
            switch(light.Type)
            {
                case LightType.Directional :
                    {
                        DirectionalLightsManager.Register(light, castShadows, castCookie);
                    }
                    break;

                case LightType.Spot :
                    {
                        SpotLightsManager.Register(light, castShadows, castCookie);
                    }
                    break;

                case LightType.Point :
                    {
                        PointLightsManager.Register(light, castShadows, castCookie);
                    }
                    break;
            }
        }

        /// <summary>
        /// Unregisters an AuraLight from the correct manager
        /// </summary>
        /// <param name="light">The candidate light</param>
        public void Unregister(AuraLight light)
        {
            switch(light.Type)
            {
                case LightType.Directional :
                    {
                        DirectionalLightsManager.Unregister(light);
                    }
                    break;

                case LightType.Spot :
                    {
                        SpotLightsManager.Unregister(light);
                    }
                    break;

                case LightType.Point :
                    {
                        PointLightsManager.Unregister(light);
                    }
                    break;
            }
        }
        #endregion
    }
}
