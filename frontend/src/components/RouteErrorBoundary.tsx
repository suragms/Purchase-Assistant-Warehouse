import React from 'react';
import { BrandIdentity } from './BrandIdentity';
export class RouteErrorBoundary extends React.Component<{ children: React.ReactNode }, { failed: boolean }> {
  state = { failed: false };
  static getDerivedStateFromError() { return { failed: true }; }
  componentDidCatch(error: Error) { console.error('Page failed to render', error); }
  render() {
    if (this.state.failed) return <div role="alert" className="p-6 space-y-3"><BrandIdentity /><h1 className="text-xl font-bold">This page could not be opened</h1><p>Your data has not been changed. Reload to try again.</p><button className="px-4 py-3 rounded bg-emerald-800 text-white" onClick={() => window.location.reload()}>Reload page</button><a className="block underline" href="/dashboard">Return to dashboard</a></div>;
    return this.props.children;
  }
}
