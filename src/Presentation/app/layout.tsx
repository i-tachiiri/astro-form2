import '../globals.css';
import { ReactNode } from 'react';
import Header from '../components/Header';
import AccessLogger from '../components/AccessLogger';

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="ja">
      <body>
        <AccessLogger />
        <Header />
        <main>{children}</main>
      </body>
    </html>
  );
}
