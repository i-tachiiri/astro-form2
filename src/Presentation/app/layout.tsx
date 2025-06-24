import '../globals.css';
import { ReactNode } from 'react';
import Header from '../components/Header';

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="ja">
      <body>
        <Header />
        <main>{children}</main>
      </body>
    </html>
  );
}
