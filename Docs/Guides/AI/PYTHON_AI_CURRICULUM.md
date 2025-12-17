# 🐍 Python & AI Learning Curriculum

**From C# Developer to AI Engineer**  
**Duration: 6 Months | Time: 15-20 hours/week**

---

## 📖 Prerequisites

### What You Already Know (C#/.NET)
✅ Object-oriented programming  
✅ Data structures and algorithms  
✅ API development (REST)  
✅ Dependency injection  
✅ Async/await patterns  
✅ Unit testing  

### What You'll Learn (Python/AI)
🎯 Python syntax and idioms  
🎯 NumPy, pandas for data manipulation  
🎯 Machine learning fundamentals  
🎯 Audio signal processing  
🎯 Natural language processing  
🎯 Cloud deployment (AWS)

---

## 🎓 Comprehensive Python Learning Resources

### **For C# Developers** (START HERE!)

**1. Python for C# Developers - Real Python**
- URL: https://realpython.com/python-vs-csharp/
- Direct C# → Python comparisons
- Time: 30 minutes
- ⭐ **Read this first!**

**2. Python Quick Reference for C# Developers**
- URL: https://github.com/cimryan/csharp-to-python
- Side-by-side syntax comparison
- Time: 1 hour

---

### **Python Fundamentals**

**3. Python Crash Course by Eric Matthes**
- Online: https://ehmatthes.github.io/pcc/
- Book: ~$30 (highly recommended!)
- Best beginner book, project-based
- Focus: Chapters 1-10

**4. Corey Schafer's Python Tutorials (YouTube)**
- URL: https://www.youtube.com/c/Coreyms
- Playlist: "Python Programming Beginner Tutorials"
- Clear explanations, perfect pacing
- Time: ~8 hours
- ⭐ **Highly recommended!**

**5. Official Python Tutorial**
- URL: https://docs.python.org/3/tutorial/
- Authoritative reference
- Time: 10 hours

---

### **Python for Data Science** (Essential for AI)

**6. Python Data Science Handbook by Jake VanderPlas**
- URL: https://jakevdp.github.io/PythonDataScienceHandbook/
- Free online!
- NumPy, pandas, matplotlib
- Time: 12 hours
- ⭐ **Essential reference!**

**7. Python for Data Analysis by Wes McKinney**
- Creator of pandas
- URL: https://wesmckinney.com/book/
- Time: 15 hours (Chapters 1-8)

**8. Kaggle's Python Course**
- URL: https://www.kaggle.com/learn/python
- Interactive, immediate feedback
- Time: 5 hours
- Free + includes pandas mini-course

---

### **Interactive Learning**

**9. Python Tutor (Visualize Execution)**
- URL: https://pythontutor.com/
- See code execute line-by-line
- ⭐ **Game changer for understanding!**

**10. Exercism - Python Track**
- URL: https://exercism.org/tracks/python
- Practice with mentor feedback
- Free

**11. LeetCode (Easy Problems)**
- URL: https://leetcode.com/problemset/
- Daily practice: 30 min
- Free tier sufficient

---

### **Video Courses**

**12. Automate the Boring Stuff with Python**
- URL: https://automatetheboringstuff.com/
- Free online + YouTube
- Practical, project-based
- Time: 10 hours

**13. Python for Everybody - Coursera**
- URL: https://www.coursera.org/specializations/python
- University of Michigan
- Free to audit
- Time: 32 hours

---

### **Quick References** (Bookmark!)

**14. Python Cheat Sheet**
- URL: https://www.pythoncheatsheet.org/
- Quick syntax lookup

**15. Official Python Docs**
- URL: https://docs.python.org/3/library/
- Built-in functions reference

---

### **YouTube Channels**

- **Corey Schafer**: Fundamentals, clear explanations
- **Tech With Tim**: Project-based learning
- **ArjanCodes**: Clean code, design patterns (advanced)
- **mCoding**: Deep Python concepts (advanced)

---

## 🎯 Your Recommended Learning Path

### **Week 1 Foundation:**
1. **Day 1-2:** Read "Python for C# Developers" + Watch Corey Schafer videos (Chapters 1-10)
2. **Day 3-4:** Python Data Science Handbook (Chapter 2 - NumPy)
3. **Day 5-7:** Python Data Science Handbook (Chapter 3 - pandas) + Code exercises

### **Ongoing:**
- **Daily:** One LeetCode easy problem (15 min)
- **When stuck:** Python Cheat Sheet + Official docs
- **Weekend:** One chapter from Python Crash Course

---

## 💰 Budget Options

**$0 Budget (Sufficient!):**
- Corey Schafer (YouTube)
- Python Data Science Handbook (free online)
- Official Python Tutorial
- Kaggle courses

**$30 Budget (Recommended):**
- Python Crash Course book ($30)

**$100 Budget (Optimal):**
- Python Crash Course ($30)
- Hands-On Machine Learning ($50)
- Python for Data Analysis ($40)

---

## 🚨 Avoid Tutorial Hell!

**Don't:**
- ❌ Watch 50 hours of tutorials before coding
- ❌ Try to learn Python "perfectly"
- ❌ Buy 10 different courses

**Do:**
- ✅ Watch 8-10 hours of fundamentals
- ✅ Start coding real project (Week 1, Day 3)
- ✅ Learn by doing
- ✅ Google errors and read Stack Overflow

**Remember:** You're a developer! Python is just different syntax for concepts you already know.  

---

## 📅 Month 1: Python + ML Fundamentals

### Week 1: Python Crash Course (C# → Python Translation)

#### Day 1-2: Syntax & Data Structures

**Python equivalents to C# concepts:**

```python
# C#: var numbers = new List<int> { 1, 2, 3, 4, 5 };
numbers = [1, 2, 3, 4, 5]  # Python list

# C#: var person = new { Name = "Helder", Age = 30 };
person = {"name": "Helder", "age": 30}  # Python dict

# C#: var even = numbers.Where(x => x % 2 == 0).ToList();
even = [x for x in numbers if x % 2 == 0]  # List comprehension

# C#: var squared = numbers.Select(x => x * x).ToList();
squared = [x * x for x in numbers]  # List comprehension

# C#: foreach (var num in numbers) { Console.WriteLine(num); }
for num in numbers:
    print(num)  # Python loop

# C#: public class Song { public string Title { get; set; } }
class Song:
    def __init__(self, title):
        self.title = title  # Python class
```

**Exercise 1: Song Catalog Manager**
```python
"""
Create a Song class and manage a collection
Concepts: Classes, lists, dictionaries, file I/O
"""

class Song:
    def __init__(self, title, artist, genre, bpm):
        self.title = title
        self.artist = artist
        self.genre = genre
        self.bpm = bpm
    
    def __repr__(self):
        return f"Song('{self.title}' by {self.artist})"

class SongCatalog:
    def __init__(self):
        self.songs = []
    
    def add_song(self, song):
        """Add a song to catalog"""
        self.songs.append(song)
    
    def find_by_genre(self, genre):
        """Find all songs of a specific genre"""
        return [song for song in self.songs if song.genre == genre]
    
    def find_by_bpm_range(self, min_bpm, max_bpm):
        """Find songs within BPM range"""
        return [
            song for song in self.songs 
            if min_bpm <= song.bpm <= max_bpm
        ]

# Usage
catalog = SongCatalog()
catalog.add_song(Song("Imagine", "John Lennon", "Rock", 76))
catalog.add_song(Song("Billie Jean", "Michael Jackson", "Pop", 117))

rock_songs = catalog.find_by_genre("Rock")
print(rock_songs)

# YOUR TASK: Add methods for:
# 1. Save catalog to CSV file
# 2. Load catalog from CSV file
# 3. Search by artist
# 4. Get songs sorted by BPM
```

**Exercise 2: CSV Processing**
```python
"""
Learn: File I/O, CSV handling
Build: Load your existing song database
"""

import csv

def load_songs_from_csv(filename):
    """Load songs from CSV file"""
    songs = []
    with open(filename, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        for row in reader:
            song = Song(
                title=row['title'],
                artist=row['artist'],
                genre=row['genre'],
                bpm=int(row['bpm'])
            )
            songs.append(song)
    return songs

def save_songs_to_csv(songs, filename):
    """Save songs to CSV file"""
    with open(filename, 'w', encoding='utf-8', newline='') as f:
        fieldnames = ['title', 'artist', 'genre', 'bpm']
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        
        writer.writeheader()
        for song in songs:
            writer.writerow({
                'title': song.title,
                'artist': song.artist,
                'genre': song.genre,
                'bpm': song.bpm
            })

# YOUR TASK: Create a CSV file with 20 songs from your catalog
# Load it, filter by genre, save results to new CSV
```

---

#### Day 3-4: NumPy Fundamentals

```python
"""
NumPy: The foundation of all scientific Python
Think of it as LINQ for numerical data
"""

import numpy as np

# Creating arrays (like C# arrays but with superpowers)
arr = np.array([1, 2, 3, 4, 5])
matrix = np.array([[1, 2, 3], [4, 5, 6]])

# Operations are vectorized (no loops needed!)
# C#: var doubled = numbers.Select(x => x * 2).ToArray();
doubled = arr * 2  # NumPy does this in C, super fast

# Boolean indexing (like LINQ Where)
# C#: var big = numbers.Where(x => x > 3).ToArray();
big = arr[arr > 3]  # Returns [4, 5]

# Aggregations
print(arr.mean())  # Average
print(arr.std())   # Standard deviation
print(arr.sum())   # Sum
```

**Exercise 3: BPM Analysis**
```python
"""
Analyze BPM distribution in your song catalog
Learn: NumPy arrays, statistical operations
"""

import numpy as np

def analyze_bpm_distribution(songs):
    """Analyze BPM statistics for song collection"""
    # Extract BPMs into NumPy array
    bpms = np.array([song.bpm for song in songs])
    
    # Calculate statistics
    stats = {
        'mean': np.mean(bpms),
        'median': np.median(bpms),
        'std': np.std(bpms),
        'min': np.min(bpms),
        'max': np.max(bpms),
        'quartiles': np.percentile(bpms, [25, 50, 75])
    }
    
    return stats

# YOUR TASK:
# 1. Find songs with BPM > (mean + 1 std deviation)
# 2. Create BPM bins: slow (<80), medium (80-120), fast (>120)
# 3. Calculate percentage of songs in each bin
# 4. Find closest song to median BPM
```

---

#### Day 5-7: Pandas for Data Analysis

```python
"""
Pandas: Like LINQ + SQL for Python
Perfect for structured data (your song catalog!)
"""

import pandas as pd

# Create DataFrame from your songs
df = pd.DataFrame([
    {'title': 'Imagine', 'artist': 'John Lennon', 'genre': 'Rock', 'bpm': 76},
    {'title': 'Billie Jean', 'artist': 'M. Jackson', 'genre': 'Pop', 'bpm': 117},
    # ... more songs
])

# Filtering (like C# Where)
rock_songs = df[df['genre'] == 'Rock']

# Grouping (like C# GroupBy)
by_genre = df.groupby('genre')['bpm'].mean()

# Sorting (like C# OrderBy)
sorted_df = df.sort_values('bpm', ascending=False)

# Adding calculated columns
df['bpm_category'] = df['bpm'].apply(lambda x: 
    'slow' if x < 80 else 'medium' if x < 120 else 'fast'
)
```

**Exercise 4: Song Catalog Analytics**
```python
"""
Build a comprehensive analytics tool
Learn: Pandas operations, data aggregation
"""

import pandas as pd
import numpy as np

class SongAnalytics:
    def __init__(self, csv_path):
        self.df = pd.read_csv(csv_path)
    
    def top_artists(self, n=10):
        """Find artists with most songs in catalog"""
        return self.df['artist'].value_counts().head(n)
    
    def genre_distribution(self):
        """Get percentage of songs per genre"""
        counts = self.df['genre'].value_counts()
        percentages = (counts / len(self.df)) * 100
        return percentages.round(2)
    
    def bpm_by_genre(self):
        """Average BPM for each genre"""
        return self.df.groupby('genre')['bpm'].agg(['mean', 'std', 'count'])
    
    def find_similar_bpm(self, target_bpm, tolerance=5):
        """Find songs with similar BPM"""
        mask = (self.df['bpm'] >= target_bpm - tolerance) & \
               (self.df['bpm'] <= target_bpm + tolerance)
        return self.df[mask]

# YOUR TASK:
# 1. Add method to find genre transitions (popular cross-genre artists)
# 2. Detect outliers (songs very different from genre average)
# 3. Find "bridge songs" (similar BPM across different genres)
# 4. Export analytics report to CSV
```

---

### Week 2: Feature Engineering

#### Day 1-2: Understanding Features

```python
"""
Features: Converting raw data into ML-friendly format
Think: Extracting meaningful information for similarity
"""

import pandas as pd
import numpy as np

class SongFeatureExtractor:
    def __init__(self):
        self.genres = []  # Will store unique genres
    
    def fit(self, songs_df):
        """Learn vocabulary from data (like fitting a model)"""
        self.genres = sorted(songs_df['genre'].unique())
    
    def one_hot_encode_genre(self, genre):
        """Convert genre to binary vector
        
        Example: If genres = ['Pop', 'Rock', 'Jazz']
        'Rock' becomes [0, 1, 0]
        """
        vector = np.zeros(len(self.genres))
        if genre in self.genres:
            idx = self.genres.index(genre)
            vector[idx] = 1
        return vector
    
    def normalize_bpm(self, bpm):
        """Scale BPM to 0-1 range (for fair comparison)
        
        Why? So BPM=120 doesn't dominate genre similarity
        """
        min_bpm = 40  # Typical minimum
        max_bpm = 200  # Typical maximum
        return (bpm - min_bpm) / (max_bpm - min_bpm)
    
    def extract_features(self, song):
        """Convert song to feature vector"""
        # Genre one-hot encoding
        genre_features = self.one_hot_encode_genre(song['genre'])
        
        # Normalized BPM
        bpm_feature = self.normalize_bpm(song['bpm'])
        
        # Combine all features
        features = np.concatenate([genre_features, [bpm_feature]])
        return features

# Example usage
extractor = SongFeatureExtractor()
extractor.fit(df)  # Learn from your catalog

song1_features = extractor.extract_features(df.iloc[0])
print(f"Feature vector: {song1_features}")
print(f"Shape: {song1_features.shape}")
```

**Exercise 5: Advanced Feature Engineering**
```python
"""
Build more sophisticated features
Learn: Domain knowledge → features
"""

class AdvancedFeatureExtractor(SongFeatureExtractor):
    def extract_tempo_category(self, bpm):
        """Categorical tempo features"""
        categories = {
            'very_slow': 1 if bpm < 60 else 0,
            'slow': 1 if 60 <= bpm < 90 else 0,
            'moderate': 1 if 90 <= bpm < 120 else 0,
            'fast': 1 if 120 <= bpm < 150 else 0,
            'very_fast': 1 if bpm >= 150 else 0
        }
        return np.array(list(categories.values()))
    
    def extract_artist_popularity(self, artist, df):
        """How many songs by this artist in catalog"""
        count = len(df[df['artist'] == artist])
        # Normalize by log (popular artists don't dominate)
        return np.log1p(count)
    
    def extract_features(self, song, df):
        """Enhanced feature extraction"""
        base_features = super().extract_features(song)
        tempo_features = self.extract_tempo_category(song['bpm'])
        popularity_feature = self.extract_artist_popularity(song['artist'], df)
        
        return np.concatenate([
            base_features,
            tempo_features,
            [popularity_feature]
        ])

# YOUR TASK:
# 1. Add feature for "decade" (extract from year if you have it)
# 2. Add feature for "artist genre diversity" (how many genres artist spans)
# 3. Add feature for "energy level" (combine BPM + genre characteristics)
# 4. Test: Do your features make sense? Plot them!
```

---

#### Day 3-5: Similarity Metrics

```python
"""
Similarity: How do we measure if two songs are similar?
Key concept: Distance in feature space
"""

import numpy as np
from scipy.spatial.distance import cosine, euclidean

def cosine_similarity(vec1, vec2):
    """
    Cosine similarity: Measures angle between vectors
    Range: -1 (opposite) to 1 (identical)
    
    Good for: When magnitude doesn't matter (genre similarity)
    """
    dot_product = np.dot(vec1, vec2)
    magnitude = np.linalg.norm(vec1) * np.linalg.norm(vec2)
    return dot_product / magnitude

def euclidean_distance(vec1, vec2):
    """
    Euclidean distance: Straight-line distance
    Range: 0 (identical) to infinity (very different)
    
    Good for: When magnitude matters (BPM difference)
    """
    return np.sqrt(np.sum((vec1 - vec2) ** 2))

# Example: Compare two songs
song1 = np.array([1, 0, 0, 0.5])  # Rock, BPM=100 (normalized)
song2 = np.array([0, 1, 0, 0.6])  # Pop, BPM=120 (normalized)
song3 = np.array([1, 0, 0, 0.52]) # Rock, BPM=104 (normalized)

print(f"Cosine similarity (song1 vs song2): {cosine_similarity(song1, song2):.3f}")
print(f"Cosine similarity (song1 vs song3): {cosine_similarity(song1, song3):.3f}")
# song3 is more similar to song1 (same genre, close BPM)
```

**Exercise 6: Build Similarity Calculator**
```python
"""
Create a tool to find similar songs
Learn: Practical application of similarity metrics
"""

class SongSimilarityCalculator:
    def __init__(self, feature_extractor):
        self.extractor = feature_extractor
        self.feature_matrix = None
        self.songs_df = None
    
    def fit(self, songs_df):
        """Pre-compute features for all songs"""
        self.songs_df = songs_df
        self.extractor.fit(songs_df)
        
        # Extract features for all songs
        features_list = []
        for idx, song in songs_df.iterrows():
            features = self.extractor.extract_features(song)
            features_list.append(features)
        
        self.feature_matrix = np.array(features_list)
    
    def find_similar(self, song_idx, n=5):
        """Find n most similar songs to song at index song_idx"""
        target_features = self.feature_matrix[song_idx]
        
        # Calculate similarity with all other songs
        similarities = []
        for idx, features in enumerate(self.feature_matrix):
            if idx != song_idx:  # Don't compare with itself
                sim = cosine_similarity(target_features, features)
                similarities.append((idx, sim))
        
        # Sort by similarity (highest first)
        similarities.sort(key=lambda x: x[1], reverse=True)
        
        # Return top n
        top_indices = [idx for idx, sim in similarities[:n]]
        return self.songs_df.iloc[top_indices]

# YOUR TASK:
# 1. Add weighted similarity (genre 70%, BPM 30%)
# 2. Add "diversity" parameter (avoid too many songs from same artist)
# 3. Add explanation (why these songs are similar)
# 4. Test with your real song catalog
```

---

#### Day 6-7: First Recommender System

```python
"""
Content-Based Recommender: Recommend based on song features
This is your first real ML system!
"""

import pandas as pd
import numpy as np

class ContentBasedRecommender:
    def __init__(self):
        self.calculator = None
        self.songs_df = None
    
    def fit(self, songs_df):
        """Train recommender (learn feature patterns)"""
        self.songs_df = songs_df
        
        # Set up feature extraction and similarity
        extractor = AdvancedFeatureExtractor()
        self.calculator = SongSimilarityCalculator(extractor)
        self.calculator.fit(songs_df)
        
        print(f"✅ Trained on {len(songs_df)} songs")
    
    def recommend(self, song_title, n=5):
        """Recommend n songs similar to given song"""
        # Find song index
        song_idx = self.songs_df[
            self.songs_df['title'] == song_title
        ].index[0]
        
        # Get similar songs
        similar = self.calculator.find_similar(song_idx, n)
        
        return similar[['title', 'artist', 'genre', 'bpm']]
    
    def recommend_for_queue(self, last_songs, n=5):
        """Recommend next songs based on recent queue
        
        Strategy: Find songs similar to recent ones, avoid repeats
        """
        # Get features of recent songs
        recent_features = []
        for title in last_songs:
            idx = self.songs_df[self.songs_df['title'] == title].index[0]
            features = self.calculator.feature_matrix[idx]
            recent_features.append(features)
        
        # Average features (what does the queue "want"?)
        target_features = np.mean(recent_features, axis=0)
        
        # Find songs similar to this average
        similarities = []
        for idx, features in enumerate(self.calculator.feature_matrix):
            song_title = self.songs_df.iloc[idx]['title']
            if song_title not in last_songs:  # Avoid repeats
                sim = cosine_similarity(target_features, features)
                similarities.append((idx, sim))
        
        similarities.sort(key=lambda x: x[1], reverse=True)
        top_indices = [idx for idx, sim in similarities[:n]]
        
        return self.songs_df.iloc[top_indices]

# Usage
recommender = ContentBasedRecommender()
recommender.fit(df)

# Get recommendations
recommendations = recommender.recommend("Imagine", n=5)
print(recommendations)
```

**Exercise 7: Test Your Recommender**
```python
"""
Evaluate your recommender system
Learn: How to know if your ML works
"""

def test_recommender(recommender, test_cases):
    """Manual testing with known songs
    
    test_cases: List of (song_title, expected_genre, expected_bpm_range)
    """
    for song, expected_genre, bpm_range in test_cases:
        print(f"\n🎵 Testing: {song}")
        recommendations = recommender.recommend(song, n=5)
        
        # Check if recommendations make sense
        genres = recommendations['genre'].unique()
        bpms = recommendations['bpm'].values
        
        print(f"  Recommended genres: {list(genres)}")
        print(f"  BPM range: {bpms.min()}-{bpms.max()}")
        
        # Manual evaluation
        if expected_genre in genres:
            print("  ✅ Genre match!")
        
        if bpm_range[0] <= bpms.mean() <= bpm_range[1]:
            print("  ✅ BPM in expected range!")

# YOUR TASK:
# 1. Create 10 test cases with different song types
# 2. Evaluate precision: How many recommendations are actually good?
# 3. Add diversity metric: Are recommendations too similar?
# 4. Document what works and what doesn't
```

---

### Week 3: scikit-learn & ML Basics

#### Day 1-3: Introduction to scikit-learn

```python
"""
scikit-learn: Industry-standard ML library
Learn the patterns, they apply to all ML frameworks
"""

from sklearn.preprocessing import StandardScaler
from sklearn.neighbors import NearestNeighbors
from sklearn.metrics.pairwise import cosine_similarity
import numpy as np

# Pattern 1: fit-transform
# (Like training a model, then using it)

scaler = StandardScaler()
scaler.fit(features)  # Learn mean and std from data
features_scaled = scaler.transform(features)  # Apply transformation

# Pattern 2: fit-predict
# (Train model, then make predictions)

knn = NearestNeighbors(n_neighbors=5, metric='cosine')
knn.fit(features_scaled)  # Learn from data
distances, indices = knn.kneighbors([new_song_features])  # Predict

# Pattern 3: Pipeline
# (Chain multiple operations)

from sklearn.pipeline import Pipeline

pipeline = Pipeline([
    ('scaler', StandardScaler()),
    ('knn', NearestNeighbors(n_neighbors=5))
])

pipeline.fit(features)
recommendations = pipeline.kneighbors([new_song_features])
```

**Exercise 8: Upgrade Recommender with sklearn**
```python
"""
Rewrite your recommender using sklearn
Learn: Industry-standard ML patterns
"""

from sklearn.preprocessing import StandardScaler
from sklearn.neighbors import NearestNeighbors
import pandas as pd

class SklearnRecommender:
    def __init__(self, n_recommendations=5):
        self.n_recommendations = n_recommendations
        self.scaler = StandardScaler()
        self.knn = NearestNeighbors(
            n_neighbors=n_recommendations + 1,  # +1 because it includes the song itself
            metric='cosine'
        )
        self.feature_matrix = None
        self.songs_df = None
    
    def _extract_features(self, songs_df):
        """Extract numerical features from songs"""
        # For now, just use BPM (you'll add more)
        features = songs_df[['bpm']].values
        return features
    
    def fit(self, songs_df):
        """Train the recommender"""
        self.songs_df = songs_df
        
        # Extract features
        self.feature_matrix = self._extract_features(songs_df)
        
        # Scale features (important for KNN!)
        self.feature_matrix = self.scaler.fit_transform(self.feature_matrix)
        
        # Fit KNN
        self.knn.fit(self.feature_matrix)
        
        print(f"✅ Trained on {len(songs_df)} songs")
    
    def recommend(self, song_title):
        """Get recommendations for a song"""
        # Find song
        song_idx = self.songs_df[
            self.songs_df['title'] == song_title
        ].index[0]
        
        # Get nearest neighbors
        distances, indices = self.knn.kneighbors(
            [self.feature_matrix[song_idx]]
        )
        
        # Remove first result (the song itself)
        indices = indices[0][1:]
        distances = distances[0][1:]
        
        # Return recommendations with similarity scores
        recommendations = self.songs_df.iloc[indices].copy()
        recommendations['similarity'] = 1 - distances  # Convert distance to similarity
        
        return recommendations[['title', 'artist', 'genre', 'bpm', 'similarity']]

# YOUR TASK:
# 1. Add more features (genre one-hot encoded)
# 2. Experiment with different metrics (euclidean vs cosine)
# 3. Add feature weighting (make genre more important than BPM)
# 4. Compare results with your custom recommender
```

---

#### Day 4-5: Evaluation Metrics

```python
"""
How do you know if your recommender is good?
Learn: Precision, recall, and practical evaluation
"""

import numpy as np

def precision_at_k(recommended, relevant, k=5):
    """
    Precision@K: How many of top K recommendations are relevant?
    
    Example:
    recommended = ['song1', 'song2', 'song3', 'song4', 'song5']
    relevant = ['song2', 'song5', 'song6']
    precision@5 = 2/5 = 0.4 (song2 and song5 are relevant)
    """
    top_k = recommended[:k]
    relevant_in_top_k = len(set(top_k) & set(relevant))
    return relevant_in_top_k / k

def recall_at_k(recommended, relevant, k=5):
    """
    Recall@K: How many relevant items did we find in top K?
    
    Using same example:
    recall@5 = 2/3 = 0.67 (found 2 out of 3 relevant songs)
    """
    top_k = recommended[:k]
    relevant_in_top_k = len(set(top_k) & set(relevant))
    return relevant_in_top_k / len(relevant)

def ndcg_at_k(recommended, relevant, k=5):
    """
    NDCG@K: Normalized Discounted Cumulative Gain
    
    Why? Position matters! 
    [relevant, relevant, irrelevant] > [irrelevant, relevant, relevant]
    
    This is industry standard for recommendation systems
    """
    dcg = 0
    for i, song in enumerate(recommended[:k]):
        if song in relevant:
            # Discount by position (later = less value)
            dcg += 1 / np.log2(i + 2)  # +2 because log2(1)=0
    
    # Ideal DCG (if we ranked perfectly)
    idcg = sum(1 / np.log2(i + 2) for i in range(min(len(relevant), k)))
    
    return dcg / idcg if idcg > 0 else 0
```

**Exercise 9: Create Evaluation Suite**
```python
"""
Build automated testing for your recommender
Learn: How ML engineers validate systems
"""

class RecommenderEvaluator:
    def __init__(self, recommender, test_data):
        """
        test_data: List of (song, expected_similar_songs)
        Example: [
            ("Imagine", ["Let It Be", "Hey Jude"]),
            ("Billie Jean", ["Thriller", "Beat It"])
        ]
        """
        self.recommender = recommender
        self.test_data = test_data
    
    def evaluate(self, k=5):
        """Run full evaluation suite"""
        precisions = []
        recalls = []
        ndcgs = []
        
        for song, relevant in self.test_data:
            # Get recommendations
            recs = self.recommender.recommend(song, n=k)
            recommended = recs['title'].tolist()
            
            # Calculate metrics
            p = precision_at_k(recommended, relevant, k)
            r = recall_at_k(recommended, relevant, k)
            n = ndcg_at_k(recommended, relevant, k)
            
            precisions.append(p)
            recalls.append(r)
            ndcgs.append(n)
        
        # Average metrics
        results = {
            'precision@5': np.mean(precisions),
            'recall@5': np.mean(recalls),
            'ndcg@5': np.mean(ndcgs)
        }
        
        return results
    
    def manual_review(self, song):
        """Print recommendations for manual inspection"""
        recs = self.recommender.recommend(song)
        print(f"\n🎵 Recommendations for '{song}':\n")
        print(recs.to_string(index=False))
        print("\n✅ Good? ❌ Bad? 🤔 Meh?")

# YOUR TASK:
# 1. Create 20 test cases manually
# 2. Run evaluation and document results
# 3. Find failure cases (where recommendations are bad)
# 4. Analyze: Is it features? Algorithm? Data?
# 5. Iterate and improve based on results
```

---

#### Day 6-7: Hyperparameter Tuning

```python
"""
Hyperparameters: Settings that affect model performance
Learn: How to optimize ML systems
"""

from sklearn.model_selection import cross_val_score
from sklearn.neighbors import NearestNeighbors
import numpy as np

def tune_k_neighbors(features, labels, k_range=range(3, 20)):
    """Find optimal number of neighbors
    
    Method: Cross-validation (test on held-out data)
    """
    results = []
    
    for k in k_range:
        knn = NearestNeighbors(n_neighbors=k, metric='cosine')
        # Score using cross-validation
        # (split data, train on some, test on others)
        scores = cross_val_score(knn, features, labels, cv=5)
        avg_score = np.mean(scores)
        
        results.append((k, avg_score))
        print(f"k={k}: score={avg_score:.3f}")
    
    # Find best k
    best_k, best_score = max(results, key=lambda x: x[1])
    return best_k, results

def tune_similarity_metric(features):
    """Compare different similarity metrics"""
    metrics = ['cosine', 'euclidean', 'manhattan']
    
    for metric in metrics:
        knn = NearestNeighbors(n_neighbors=5, metric=metric)
        # ... evaluation code ...
        print(f"{metric}: score=...")
```

**Exercise 10: Build Hyperparameter Search**
```python
"""
Find optimal settings for your recommender
Learn: Systematic experimentation
"""

class HyperparameterTuner:
    def __init__(self, recommender_class, train_data, test_data):
        self.recommender_class = recommender_class
        self.train_data = train_data
        self.test_data = test_data
    
    def grid_search(self, param_grid):
        """Try all combinations of parameters
        
        param_grid = {
            'n_neighbors': [3, 5, 10, 15],
            'metric': ['cosine', 'euclidean'],
            'feature_weights': [(0.7, 0.3), (0.5, 0.5), (0.3, 0.7)]
        }
        """
        results = []
        
        for n in param_grid['n_neighbors']:
            for metric in param_grid['metric']:
                for weights in param_grid['feature_weights']:
                    # Train with these params
                    rec = self.recommender_class(
                        n_neighbors=n,
                        metric=metric,
                        feature_weights=weights
                    )
                    rec.fit(self.train_data)
                    
                    # Evaluate
                    evaluator = RecommenderEvaluator(rec, self.test_data)
                    metrics = evaluator.evaluate()
                    
                    results.append({
                        'params': {'n': n, 'metric': metric, 'weights': weights},
                        'metrics': metrics
                    })
        
        # Find best
        best = max(results, key=lambda x: x['metrics']['ndcg@5'])
        return best, results

# YOUR TASK:
# 1. Define parameter grid with 10+ combinations
# 2. Run grid search (might take time!)
# 3. Analyze results: Which parameters matter most?
# 4. Document findings: What did you learn?
```

---

### Week 4: FastAPI & Deployment

#### Day 1-2: FastAPI Basics

```python
"""
FastAPI: Modern Python web framework
Perfect for ML APIs
"""

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List

app = FastAPI(title="Song Recommender API")

# Define request/response models (like C# DTOs)
class Song(BaseModel):
    title: str
    artist: str
    genre: str
    bpm: int

class RecommendationRequest(BaseModel):
    song_title: str
    n: int = 5

class RecommendationResponse(BaseModel):
    recommendations: List[Song]
    query_song: str

# Initialize recommender (do this once at startup)
recommender = None

@app.on_event("startup")
async def startup_event():
    """Load recommender on startup"""
    global recommender
    # Load your trained recommender
    recommender = load_recommender()
    print("✅ Recommender loaded")

# Health check endpoint
@app.get("/")
async def root():
    return {"status": "active", "service": "song-recommender"}

# Recommendation endpoint
@app.post("/recommend", response_model=RecommendationResponse)
async def get_recommendations(request: RecommendationRequest):
    """Get song recommendations"""
    if recommender is None:
        raise HTTPException(status_code=503, message="Service not ready")
    
    try:
        recs = recommender.recommend(request.song_title, n=request.n)
        
        # Convert to response format
        songs = [
            Song(
                title=row['title'],
                artist=row['artist'],
                genre=row['genre'],
                bpm=row['bpm']
            )
            for _, row in recs.iterrows()
        ]
        
        return RecommendationResponse(
            recommendations=songs,
            query_song=request.song_title
        )
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# Run with: uvicorn main:app --reload
```

**Exercise 11: Build Complete API**
```python
"""
Create production-ready API for your recommender
Learn: API design, error handling, documentation
"""

from fastapi import FastAPI, HTTPException, File, UploadFile
from fastapi.middleware.cors import CORSMiddleware
import pandas as pd

app = FastAPI(
    title="MyVocaList Recommender API",
    description="AI-powered song recommendations",
    version="1.0.0"
)

# Enable CORS for MAUI app
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Configure properly in production
    allow_methods=["*"],
    allow_headers=["*"],
)

# Endpoints to implement:

@app.get("/songs")
async def list_songs(
    genre: str = None,
    min_bpm: int = None,
    max_bpm: int = None,
    limit: int = 100
):
    """List songs with filters"""
    # YOUR CODE HERE
    pass

@app.get("/songs/{song_id}/similar")
async def similar_songs(song_id: str, n: int = 5):
    """Find similar songs by ID"""
    # YOUR CODE HERE
    pass

@app.post("/recommend/batch")
async def batch_recommend(song_ids: List[str]):
    """Get recommendations for multiple songs"""
    # YOUR CODE HERE
    pass

@app.get("/analytics/genres")
async def genre_analytics():
    """Get genre distribution"""
    # YOUR CODE HERE
    pass

# YOUR TASK:
# 1. Implement all endpoints
# 2. Add proper error handling
# 3. Add request validation
# 4. Test with Postman/curl
# 5. Generate API documentation (automatic with FastAPI!)
```

---

#### Day 3-4: Testing & Optimization

```python
"""
Testing: Ensure your API works correctly
Learn: Professional API development practices
"""

import pytest
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_root():
    """Test health check endpoint"""
    response = client.get("/")
    assert response.status_code == 200
    assert response.json()["status"] == "active"

def test_recommendations_valid():
    """Test recommendations with valid song"""
    response = client.post(
        "/recommend",
        json={"song_title": "Imagine", "n": 5}
    )
    assert response.status_code == 200
    data = response.json()
    assert len(data["recommendations"]) == 5
    assert data["query_song"] == "Imagine"

def test_recommendations_invalid_song():
    """Test error handling for invalid song"""
    response = client.post(
        "/recommend",
        json={"song_title": "NonexistentSong", "n": 5}
    )
    assert response.status_code == 400

def test_recommendations_edge_cases():
    """Test edge cases"""
    # n=0
    response = client.post("/recommend", json={"song_title": "Imagine", "n": 0})
    assert response.status_code == 400
    
    # n > catalog size
    response = client.post("/recommend", json={"song_title": "Imagine", "n": 10000})
    assert response.status_code == 200

# Run tests with: pytest test_main.py
```

**Exercise 12: Performance Optimization**
```python
"""
Make your API fast and efficient
Learn: Performance profiling and optimization
"""

import time
import cProfile
from functools import lru_cache

# Optimization 1: Caching
@lru_cache(maxsize=1000)
def get_recommendations_cached(song_title: str, n: int):
    """Cache recommendations (they don't change often)"""
    return recommender.recommend(song_title, n)

# Optimization 2: Lazy loading
class LazyRecommender:
    def __init__(self):
        self._recommender = None
    
    @property
    def recommender(self):
        if self._recommender is None:
            self._recommender = load_recommender()  # Load on first use
        return self._recommender

# Optimization 3: Batch processing
async def batch_recommendations(song_titles: List[str]):
    """Process multiple songs efficiently"""
    # Instead of calling recommender n times, vectorize
    results = recommender.batch_recommend(song_titles)
    return results

# YOUR TASK:
# 1. Profile your API (find slow parts)
# 2. Add caching where appropriate
# 3. Optimize feature extraction (vectorize operations)
# 4. Measure improvement (before/after)
# 5. Document optimization strategies
```

---

## 📚 Month 1 Deliverables

By end of Month 1, you should have:

### ✅ Code
- [ ] Working song recommender (content-based)
- [ ] Feature extraction pipeline
- [ ] FastAPI service
- [ ] Test suite (>80% coverage)
- [ ] Deployed to cloud (Render/ngrok)

### ✅ Documentation
- [ ] README with setup instructions
- [ ] API documentation (auto-generated)
- [ ] Blog post: "Building ML Recommendations"
- [ ] Jupyter notebook with experiments

### ✅ Skills
- [ ] Python fundamentals (syntax, OOP, file I/O)
- [ ] NumPy & Pandas (data manipulation)
- [ ] scikit-learn basics (pipelines, KNN)
- [ ] Feature engineering concepts
- [ ] Evaluation metrics (precision, NDCG)
- [ ] FastAPI & REST APIs

### ✅ Integration
- [ ] C# service in MAUI consuming Python API
- [ ] Error handling and fallbacks
- [ ] End-to-end testing

---

## 🔄 Daily Study Routine

```
Morning (1 hour): Theory
- Read course material / book chapter
- Watch tutorial video
- Take notes on key concepts

Afternoon (2-3 hours): Coding
- Write code yourself (no copy-paste)
- Complete daily exercise
- Debug and test

Evening (30 min): Review
- Document what you learned
- Update learning journal
- Prepare questions for next day

Weekend (4 hours): Integration
- Connect Python work with MAUI
- Test end-to-end
- Write blog post draft
```

---

## 📈 Progress Tracking

Use this checklist weekly:

**Week 1:**
- [ ] Python syntax comfortable
- [ ] Can manipulate lists/dicts without docs
- [ ] CSV processing works
- [ ] NumPy basics understood

**Week 2:**
- [ ] Feature extraction makes sense
- [ ] Can explain one-hot encoding
- [ ] Similarity metrics working
- [ ] First recommender functional

**Week 3:**
- [ ] sklearn patterns understood
- [ ] Evaluation metrics calculated
- [ ] Hyperparameter tuning attempted
- [ ] Results documented

**Week 4:**
- [ ] FastAPI endpoints working
- [ ] Tests passing
- [ ] Deployed to cloud
- [ ] MAUI integration complete

---

## 🎯 Month 2 Preview

Next month you'll learn:
- **Collaborative filtering** (user-based recommendations)
- **Matrix factorization** (advanced technique)
- **A/B testing** (evaluate changes scientifically)
- **Production deployment** (monitoring, logging)

But first, **master Month 1**! Quality over speed.

---

## 💡 Tips for Success

1. **Type everything yourself** - Don't copy-paste code
2. **Break when stuck** - 20 min rule (if stuck >20 min, ask for help)
3. **Explain to others** - Best way to find gaps in understanding
4. **Build incrementally** - Small working pieces > big broken systems
5. **Test constantly** - Don't wait until "done" to test

---

## 📞 When You Need Help

**Good questions to ask:**
- "I implemented X but getting Y result. Expected Z. Here's my code..."
- "I understand concept X but don't see how it applies to Y"
- "These two approaches seem similar. What's the difference?"

**Bad questions:**
- "It doesn't work" (no context)
- "Write the code for me" (no learning)
- "Is this right?" (show your reasoning first)

---

Ready to start? Begin with **Week 1, Day 1** tomorrow! 🚀

Next document: `AWS_SETUP_GUIDE.md` for cloud deployment when you're ready.
