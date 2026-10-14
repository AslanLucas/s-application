import { describe, expect, it } from 'vitest';
import reducer, { noteAdded, noteRemoved, notesLoaded, type Note } from './notesSlice';

const note: Note = { id: '1', title: 'Titel', content: 'Inhalt' };

describe('notesSlice', () => {
  it('starts with an empty list', () => {
    expect(reducer(undefined, { type: 'unknown' }).items).toEqual([]);
  });

  it('replaces the list when notes are loaded', () => {
    const state = reducer(undefined, notesLoaded([note]));
    expect(state.items).toEqual([note]);
  });

  it('adds a note', () => {
    const state = reducer(undefined, noteAdded(note));
    expect(state.items).toHaveLength(1);
  });

  it('removes a note by id', () => {
    const start = reducer(undefined, notesLoaded([note, { ...note, id: '2' }]));
    const state = reducer(start, noteRemoved('1'));
    expect(state.items.map((n) => n.id)).toEqual(['2']);
  });
});
