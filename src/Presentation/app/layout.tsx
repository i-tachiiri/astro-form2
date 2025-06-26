import '../globals.css';
import { ReactNode } from 'react';
import { NextUIProvider } from '@nextui-org/system';
import AccessLogger from '../components/AccessLogger';

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="ja">
      <body className="min-h-screen overflow-x-hidden">
        <NextUIProvider>
          <AccessLogger />
          {children}
        </NextUIProvider>
      </body>
    </html>
  );
}
