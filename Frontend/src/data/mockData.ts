export interface SongType {
  id: number;
  title: string;
  artist: string;
  album: string;
  duration: string;
  isLiked: boolean;
  url: string;
}

export interface PlaylistType {
  id: string; // Dùng làm ID trên URL (slug)
  title: string;
  subtitle: string;
  description: string;
  isPublic: boolean; // 🟢 QUYẾT ĐỊNH ẨN/HIỆN Ở PROFILE
  type: "playlists" | "albums";
  songs: SongType[];
}

export const MOCK_PLAYLISTS: PlaylistType[] = [
  {
    id: "daily-mix-1",
    title: "Daily Mix 1",
    subtitle: "Playlist · 3 songs",
    description: "The Weeknd, Dua Lipa and more",
    isPublic: true, // Công khai -> Sẽ hiện ở Profile
    type: "playlists",
    songs: [
      { id: 1, title: "Sunset Boulevard", artist: "Neon Coast", album: "City Lights", duration: "0:41", isLiked: true, url: "" },
      { id: 2, title: "Velvet Sky", artist: "Aria Lane", album: "Nightfall", duration: "0:45", isLiked: false, url: "" },
      { id: 3, title: "Paper Planes", artist: "The Drifters", album: "Horizons", duration: "0:50", isLiked: false, url: "" },
    ]
  },
  {
    id: "summer-2026",
    title: "Summer 2026",
    subtitle: "Playlist · 2 songs",
    description: "Chill summer vibes",
    isPublic: false, // Riêng tư -> Sẽ ẨN khỏi Profile!
    type: "playlists",
    songs: [
      { id: 4, title: "Blinding Lights", artist: "The Weeknd", album: "After Hours", duration: "3:20", isLiked: false, url: "" },
      { id: 5, title: "Starboy", artist: "The Weeknd", album: "Starboy", duration: "3:50", isLiked: false, url: "" },
    ]
  },
  {
    id: "deep-focus",
    title: "Deep Focus",
    subtitle: "Playlist · 120 songs",
    description: "Concentrate with these ambient tracks",
    isPublic: true, // Công khai -> Sẽ hiện ở Profile
    type: "playlists",
    songs: []
  },
  // {
  //   id: "liked-songs",
  //   title: "Liked Songs",
  //   subtitle: "Playlist · 0 songs",
  //   description: "Your favorite tracks",
  //   isPublic: true, // Công khai -> Sẽ hiện ở Profile
  //   type: "playlists",
  //   songs: []
  // }
];