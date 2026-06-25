import { FormEvent, useEffect, useState } from 'react';
import { profileApi } from '../api/services';
import type { UserProfile } from '../api/types';

export function ProfilePage() {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [displayName, setDisplayName] = useState('');
  const [bio, setBio] = useState('');
  const [message, setMessage] = useState('');

  useEffect(() => {
    void profileApi.get().then((p) => {
      setProfile(p);
      setDisplayName(p.displayName);
      setBio(p.bio || '');
    });
  }, []);

  const onSave = async (e: FormEvent) => {
    e.preventDefault();
    const updated = await profileApi.update({ displayName, bio });
    setProfile(updated);
    setMessage('Profile updated');
  };

  if (!profile) return <p className="text-spotify-light">Loading profile...</p>;

  return (
    <div className="max-w-lg">
      <h2 className="text-2xl font-bold mb-4">Profile</h2>
      <form onSubmit={onSave} className="space-y-3 bg-spotify-gray p-4 rounded-lg">
        <p className="text-sm text-spotify-light">Email: {profile.email}</p>
        <p className="text-sm text-spotify-light">User id: {profile.id}</p>
        <input value={displayName} onChange={(e) => setDisplayName(e.target.value)} className="w-full p-2 rounded bg-spotify-dark" />
        <textarea value={bio} onChange={(e) => setBio(e.target.value)} className="w-full p-2 rounded bg-spotify-dark" rows={4} />
        <button className="bg-spotify-green text-black px-4 py-2 rounded-full font-semibold">Save</button>
        {message && <p className="text-sm text-spotify-light">{message}</p>}
      </form>
    </div>
  );
}
