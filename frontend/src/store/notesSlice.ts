import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

export interface Note {
  id: string;
  title: string;
  content: string;
}

export interface NotesState {
  items: Note[];
}

const initialState: NotesState = { items: [] };

const notesSlice = createSlice({
  name: 'notes',
  initialState,
  reducers: {
    notesLoaded(state, action: PayloadAction<Note[]>) {
      state.items = action.payload;
    },
    noteAdded(state, action: PayloadAction<Note>) {
      state.items.push(action.payload);
    },
    noteRemoved(state, action: PayloadAction<string>) {
      state.items = state.items.filter((n) => n.id !== action.payload);
    },
  },
});

export const { notesLoaded, noteAdded, noteRemoved } = notesSlice.actions;
export default notesSlice.reducer;
