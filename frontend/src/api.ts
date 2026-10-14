import type { Note } from './store/notesSlice';

export async function fetchNotes(): Promise<Note[]> {
  const res = await fetch('/api/notes');
  if (!res.ok) throw new Error(`Notizen laden fehlgeschlagen (${res.status})`);
  return res.json();
}

export async function createNote(title: string, content: string): Promise<Note> {
  const res = await fetch('/api/notes', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, content }),
  });
  if (!res.ok) throw new Error(`Notiz anlegen fehlgeschlagen (${res.status})`);
  return res.json();
}

export async function deleteNote(id: string): Promise<void> {
  const res = await fetch(`/api/notes/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error(`Notiz löschen fehlgeschlagen (${res.status})`);
}
