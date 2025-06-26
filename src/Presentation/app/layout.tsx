import '../globals.css';
import { ReactNode } from 'react';
import { HeroUIProvider } from '@heroui/react';
import AccessLogger from '../components/AccessLogger';

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="ja">
      <body className="min-h-screen overflow-x-hidden">
        <HeroUIProvider>
          <AccessLogger />
          {children}
        </HeroUIProvider>
      </body>
    </html>
  );
}
