﻿/******************************************************************************
 * Managed3D: A 3D Graphics API for .NET and Mono - http://gearedstudios.com/ *
 * Copyright © 2009-2012 William 'cathode' Shelley. All Rights Reserved.      *
 * This software is released under the terms and conditions of the MIT/X11    *
 * license. See the 'license.txt' file for details.                           *
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Managed3D.SceneGraph;
using Managed3D.Geometry;
using Managed3D.Platform.Microsoft;
using Managed3D.Platform.Microsoft.OpenGL;

namespace Managed3D.Rendering.OpenGL
{
    /// <summary>
    /// Provides a real-time renderer that utilizes the OpenGL API.
    /// </summary>
    public class GLRenderer : Renderer, IDisposable
    {
        #region Fields
        private IntPtr deviceContext;
        private IntPtr renderingContext;
        private IntPtr windowHandle;
        private bool isDisposed;
        #endregion
        #region Constructors
        ~GLRenderer()
        {
            this.Dispose(false);
        }
        #endregion
        #region Methods
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
            this.isDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            WGL.MakeCurrent(this.deviceContext, IntPtr.Zero);
            WGL.DeleteContext(this.renderingContext);
        }

        public override void Start()
        {

        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        protected override void OnInitializing(RendererInitializationEventArgs e)
        {
            base.OnInitializing(e);
            
            IntPtr hinst = IntPtr.Zero;
            /*
            this.windowHandle = User32.CreateWindowEx(WindowStyle.Left,
                "GLRenderer",
                "Managed3D GLRenderer",
                0,
                0, 0,
                this.Profile.Width, this.Profile.Height,
                IntPtr.Zero,
                IntPtr.Zero,
                hinst,
                IntPtr.Zero);
            */

            GL.ShadeModel(ShadeModel.Flat);
            GL.ClearDepth(1.0);
            GL.Hint(HintTarget.PerspectiveCorrection, HintMode.Nicest);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

        protected override void OnSceneChanged(EventArgs e)
        {
            base.OnSceneChanged(e);
            var clamped = this.Scene.BackgroundColor.Clamp();
            GL.ClearColor((float)clamped.X,
                          (float)clamped.Y,
                          (float)clamped.Z,
                          (float)clamped.W);
        }

        protected override void OnPreRender(RenderEventArgs e)
        {
            base.OnPreRender(e);

            GL.Clear(GL.GL_COLOR_BUFFER_BIT);
            GL.LoadIdentity();
        }

        protected override void OnRender(RenderEventArgs e)
        {
            base.OnRender(e);

            if (this.Scene == null || this.Scene.Root == null)
                return;

            GL.Rotate(30, 1.0, 0, 0);

            this.ProcessNode(this.Scene.Root);

        }

        protected override void OnProfileChanged(EventArgs e)
        {
            base.OnProfileChanged(e);

            GL.Viewport(0, 0, this.Profile.Width, this.Profile.Height);
            GL.MatrixMode(MatrixMode.Projection);

            GLU.Perspective(45.0, (double)this.Profile.Width / (double)this.Profile.Height, 0.1, 100.0);
            GL.MatrixMode(MatrixMode.ModelView);
            GL.LoadIdentity();
        }

        protected override void OnPostRender(RenderEventArgs e)
        {
            base.OnPostRender(e);

           
        }

        protected virtual void ProcessNode(Node node)
        {
            //var axis = node.Orientation.Axis;
            //GL.Rotate(node.Orientation.Angle.Degrees, axis.X, axis.Y, axis.Z);
            GL.Translate(node.Position.X, node.Position.Y, node.Position.Z);

            if (node is GeometryNode)
            {
                var mesh = ((GeometryNode)node).Geometry;
                foreach (var poly in mesh.Polygons)
                {
                    GL.Begin(BeginMode.Polygon);
                    foreach (var vert in poly.Vertices)
                        GL.Vertex3(vert.X, vert.Y, vert.Z);

                    GL.End();
                }
            }
        }

        private void IdleFunc_Callback()
        {
            GLUT.PostRedisplay();
            return;
        }

        private void DisplayFunc_Callback()
        {
            this.RenderFrame();
            return;
        }

        #endregion
    }
}