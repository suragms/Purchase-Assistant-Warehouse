import { Loader2 } from 'lucide-react';
import logoUrl from '../../../brand/logo.webp';

export function BrandLogo({ className = 'h-10 w-12' }: { className?: string }) {
  return <img src={logoUrl} width={235} height={195} alt="Harisree Agency logo"
    className={`shrink-0 object-contain rounded ${className}`} />;
}

export function BrandIdentity({ className = '', inverse = false, logoClassName }: {
  className?: string;
  inverse?: boolean;
  logoClassName?: string;
}) {
  return (
    <div className={`flex min-w-0 items-center gap-2 ${className}`}>
      <BrandLogo className={logoClassName} />
      <div className="min-w-0">
        <p className={`truncate text-sm font-bold leading-tight ${inverse ? 'text-white' : 'text-[#0F172A]'}`} title="Warehouse Assistant">Warehouse Assistant</p>
        <p className={`truncate text-xs leading-tight ${inverse ? 'text-[#8FC4BC]' : 'text-[#475569]'}`}>Harisree Agency</p>
      </div>
    </div>
  );
}

export function BrandLoading({ message = 'Loading page…', fullScreen = false }: { message?: string; fullScreen?: boolean }) {
  return (
    <div role="status" className={`flex flex-col items-center justify-center gap-4 bg-[#F7F9F6] ${fullScreen ? 'min-h-screen min-h-[100svh] w-full p-8' : 'p-8'}`}>
      <BrandIdentity />
      <Loader2 aria-hidden="true" className="h-8 w-8 animate-spin text-[#0E4F46] motion-reduce:animate-none" />
      <p className="text-sm">{message}</p>
    </div>
  );
}
