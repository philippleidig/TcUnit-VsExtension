﻿using System;
using System.ComponentModel.Composition;
using TcUnit.VisualStudio.EventWatchers.EventArgs;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TcUnit.VisualStudio;
using TcUnit.EventWatchers;

namespace TcUnit.VisualStudio.EventWatchers
{
	[Export(typeof(ISolutionEventsListener))]
    public class SolutionEventsListener : IVsSolutionEvents, ISolutionEventsListener
    {
        private readonly IVsSolution solution;
        private uint cookie = VSConstants.VSCOOKIE_NIL;

        /// <summary>
        /// Fires an event when a project is opened/closed/loaded/unloaded
        /// </summary>
        public event EventHandler<SolutionEventsListenerEventArgs> SolutionProjectChanged;

        public event EventHandler SolutionUnloaded;

        [ImportingConstructor]
        public SolutionEventsListener([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider)
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			this.solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
        }

        public void StartListeningForChanges()
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			if (this.solution != null)
            {
                int hr = this.solution.AdviseSolutionEvents(this, out cookie);
                ErrorHandler.ThrowOnFailure(hr); // do nothing if this fails
            }
        }

        public void StopListeningForChanges()
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			if (this.cookie != VSConstants.VSCOOKIE_NIL && this.solution != null)
            {
                int hr = this.solution.UnadviseSolutionEvents(cookie);
                ErrorHandler.Succeeded(hr); // do nothing if this fails

                this.cookie = VSConstants.VSCOOKIE_NIL;
            }
        }

        public void OnSolutionProjectUpdated(IVsProject project, SolutionChangedReason reason)
        {
            if (SolutionProjectChanged != null && project != null)
            {
                SolutionProjectChanged(this, new SolutionEventsListenerEventArgs(project, reason));
            }
        }

        public void OnSolutionUnloaded()
        {
            if(SolutionUnloaded != null)
            {
                SolutionUnloaded(this, new System.EventArgs());
            }
        }


        /// <summary>
        /// This event is called when a project has been reloaded. This happens when you choose to unload a project 
        /// (often to edit its .proj file) and then reload it.
        /// </summary>
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			var project = pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Load);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This gets called when a project is unloaded
        /// </summary>
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			var project = pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Unload);
            return VSConstants.S_OK;
        }

	    public int OnAfterCloseSolution(object pUnkReserved)
	    {
	        OnSolutionUnloaded();
            return VSConstants.S_OK;
        }

        // Unused events...

        /// <summary>
        /// This gets called when a project is opened
        /// </summary>
        /// <param name="pHierarchy"></param>
        /// <param name="fAdded">0 if alreay part of solution, 1 if it is being added to the solution</param>
        /// <returns></returns>
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }
    }

}