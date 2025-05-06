import LinkIcon from '../assets/icons/link.svg?react';
import LinkOffIcon from '../assets/icons/link-off.svg?react';

type Props = {
  isConnected: boolean;
};

export default function ConnectionStatus({ isConnected }: Props) {
  const Icon = isConnected ? LinkIcon : LinkOffIcon;
  const color = isConnected ? 'green' : 'red';
  const label = isConnected ? 'Connected' : 'Disconnected';

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
      <Icon style={{ width: 20, height: 20, color }} />
      <span style={{ color }}>{label}</span>
    </div>
  );
}
