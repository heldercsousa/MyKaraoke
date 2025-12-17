# 📅 Week-by-Week Implementation Plan

**Practical Daily Tasks for Month 1**  
**Goal: Working Recommendation System + API + MAUI Integration**

---

## 🎯 Overview

This document breaks down Month 1 into **actionable daily tasks**. Each task is designed to take 2-3 hours, with clear success criteria.

---

## 📆 Week 1: Python Foundations (Dec 16-22)

### **Monday - Day 1: Environment Setup + Python Basics**

**Time: 2-3 hours**

**Morning (1h): Setup**
```bash
# Task 1: Install Python 3.11
python --version  # Verify: 3.11.x

# Task 2: Create virtual environment
python -m venv myvocalist-ai
source myvocalist-ai/bin/activate  # Windows: Scripts\activate

# Task 3: Install core packages
pip install pandas numpy jupyter

# Task 4: Verify installations
python -c "import pandas; print(pandas.__version__)"
python -c "import numpy; print(numpy.__version__)"
```

**Afternoon (1-2h): First Python Code**
```python
# Task 5: Create songs.csv
# Create file with 10 songs from your MyVocaList database
# Columns: title,artist,genre,bpm

# Task 6: Write first script (load_songs.py)
import pandas as pd

# Load CSV
df = pd.read_csv('songs.csv')

# Display first 5 songs
print(df.head())

# Basic statistics
print(f"Total songs: {len(df)}")
print(f"Genres: {df['genre'].unique()}")
print(f"Average BPM: {df['bpm'].mean():.1f}")

# Filter songs by genre
rock_songs = df[df['genre'] == 'Rock']
print(f"Rock songs: {len(rock_songs)}")

# YOUR SUCCESS CRITERIA:
# ✅ Script runs without errors
# ✅ Prints song statistics correctly
# ✅ Can filter by any genre
```

**Evening (30min): Documentation**
```markdown
# Create: learning_journal.md
# Week 1, Day 1

## What I learned:
- Python syntax basics (no semicolons!)
- Pandas DataFrames (like C# DataTables but better)
- Virtual environments (isolated package management)

## Challenges:
- [Write what was difficult]

## Questions for tomorrow:
- [List any confusion]

## Code highlights:
[Paste your most interesting code snippet]
```

**✅ Success Criteria:**
- [ ] Python 3.11 installed
- [ ] Virtual environment created and activated
- [ ] Can load and filter CSV with pandas
- [ ] Learning journal started

---

### **Tuesday - Day 2: Data Manipulation**

**Time: 2-3 hours**

```python
# Task 1: Create song_analyzer.py
import pandas as pd
import numpy as np

class SongAnalyzer:
    def __init__(self, csv_path):
        self.df = pd.read_csv(csv_path)
    
    def get_genre_stats(self):
        """Calculate statistics per genre"""
        stats = self.df.groupby('genre').agg({
            'bpm': ['mean', 'std', 'count']
        })
        return stats
    
    def find_outliers(self, column='bpm'):
        """Find songs with unusual BPM"""
        mean = self.df[column].mean()
        std = self.df[column].std()
        
        outliers = self.df[
            (self.df[column] < mean - 2*std) |
            (self.df[column] > mean + 2*std)
        ]
        return outliers
    
    def find_similar_bpm(self, target_bpm, tolerance=10):
        """Find songs with similar BPM"""
        mask = (self.df['bpm'] >= target_bpm - tolerance) & \
               (self.df['bpm'] <= target_bpm + tolerance)
        return self.df[mask]

# Task 2: Test your analyzer
analyzer = SongAnalyzer('songs.csv')

print("📊 Genre Statistics:")
print(analyzer.get_genre_stats())

print("\n🎯 Outliers:")
print(analyzer.find_outliers())

print("\n🎵 Songs similar to 120 BPM:")
print(analyzer.find_similar_bpm(120))

# Task 3: Add your own methods
# - find_by_artist(artist_name)
# - get_fastest_songs(n=5)
# - get_slowest_songs(n=5)
```

**✅ Success Criteria:**
- [ ] SongAnalyzer class works
- [ ] Can calculate genre statistics
- [ ] Can find outliers
- [ ] Added 3 custom methods

---

### **Wednesday - Day 3: NumPy & Feature Engineering**

**Time: 2-3 hours**

```python
# Task 1: Create feature_extractor.py
import numpy as np
import pandas as pd

class SimpleFeatureExtractor:
    def __init__(self):
        self.genres = []
    
    def fit(self, df):
        """Learn genre vocabulary"""
        self.genres = sorted(df['genre'].unique())
        print(f"✅ Learned {len(self.genres)} genres: {self.genres}")
    
    def one_hot_encode_genre(self, genre):
        """Convert genre to binary vector
        
        Example:
        genres = ['Pop', 'Rock', 'Jazz']
        'Rock' -> [0, 1, 0]
        """
        vector = np.zeros(len(self.genres))
        if genre in self.genres:
            idx = self.genres.index(genre)
            vector[idx] = 1
        return vector
    
    def normalize_bpm(self, bpm):
        """Scale BPM to 0-1 range"""
        min_bpm = 40
        max_bpm = 200
        normalized = (bpm - min_bpm) / (max_bpm - min_bpm)
        return np.clip(normalized, 0, 1)  # Keep in 0-1 range
    
    def extract_features(self, song_row):
        """Convert song to feature vector"""
        genre_features = self.one_hot_encode_genre(song_row['genre'])
        bpm_feature = np.array([self.normalize_bpm(song_row['bpm'])])
        
        # Combine
        features = np.concatenate([genre_features, bpm_feature])
        return features

# Task 2: Test feature extraction
df = pd.read_csv('songs.csv')

extractor = SimpleFeatureExtractor()
extractor.fit(df)

# Extract features for first song
first_song = df.iloc[0]
features = extractor.extract_features(first_song)

print(f"Song: {first_song['title']}")
print(f"Features: {features}")
print(f"Feature shape: {features.shape}")

# Task 3: Extract for all songs
all_features = []
for idx, song in df.iterrows():
    features = extractor.extract_features(song)
    all_features.append(features)

feature_matrix = np.array(all_features)
print(f"\n✅ Feature matrix shape: {feature_matrix.shape}")
print(f"(rows=songs, columns=features)")
```

**✅ Success Criteria:**
- [ ] Can one-hot encode genres
- [ ] Can normalize BPM to 0-1
- [ ] Feature extraction works for all songs
- [ ] Understand why we normalize

---

### **Thursday - Day 4: Similarity Calculation**

**Time: 2-3 hours**

```python
# Task 1: Create similarity_calculator.py
import numpy as np

def cosine_similarity(vec1, vec2):
    """Calculate cosine similarity between two vectors
    
    Returns: Value between -1 and 1
    1 = identical, 0 = unrelated, -1 = opposite
    """
    dot_product = np.dot(vec1, vec2)
    magnitude1 = np.linalg.norm(vec1)
    magnitude2 = np.linalg.norm(vec2)
    
    if magnitude1 == 0 or magnitude2 == 0:
        return 0
    
    return dot_product / (magnitude1 * magnitude2)

def euclidean_distance(vec1, vec2):
    """Calculate Euclidean distance
    
    Returns: Value >= 0
    0 = identical, larger = more different
    """
    return np.sqrt(np.sum((vec1 - vec2) ** 2))

# Task 2: Test with sample vectors
# Imagine: [genre_rock, genre_pop, genre_jazz, bpm_normalized]
song1 = np.array([1, 0, 0, 0.5])  # Rock, BPM=100
song2 = np.array([1, 0, 0, 0.55]) # Rock, BPM=110
song3 = np.array([0, 1, 0, 0.8])  # Pop, BPM=160

print("Similarity Tests:")
print(f"song1 vs song2 (same genre, close BPM): {cosine_similarity(song1, song2):.3f}")
print(f"song1 vs song3 (different genre): {cosine_similarity(song1, song3):.3f}")

print("\nDistance Tests:")
print(f"song1 vs song2: {euclidean_distance(song1, song2):.3f}")
print(f"song1 vs song3: {euclidean_distance(song1, song3):.3f}")

# Task 3: Find most similar song in catalog
from feature_extractor import SimpleFeatureExtractor
import pandas as pd

df = pd.read_csv('songs.csv')
extractor = SimpleFeatureExtractor()
extractor.fit(df)

# Extract all features
feature_matrix = np.array([
    extractor.extract_features(row) 
    for _, row in df.iterrows()
])

# Pick a target song
target_idx = 0
target_features = feature_matrix[target_idx]

# Calculate similarity with all other songs
similarities = []
for idx in range(len(feature_matrix)):
    if idx != target_idx:
        sim = cosine_similarity(target_features, feature_matrix[idx])
        similarities.append((idx, sim))

# Sort by similarity
similarities.sort(key=lambda x: x[1], reverse=True)

# Show top 3
print(f"\nMost similar to '{df.iloc[target_idx]['title']}':")
for idx, sim in similarities[:3]:
    song = df.iloc[idx]
    print(f"  {song['title']} by {song['artist']} - Similarity: {sim:.3f}")
```

**✅ Success Criteria:**
- [ ] Understand cosine similarity
- [ ] Can calculate similarity between vectors
- [ ] Found similar songs in your catalog
- [ ] Similarity scores make sense

---

### **Friday - Day 5: First Recommender System**

**Time: 3 hours**

```python
# Task 1: Create simple_recommender.py
import numpy as np
import pandas as pd
from feature_extractor import SimpleFeatureExtractor

class SimpleRecommender:
    def __init__(self):
        self.extractor = SimpleFeatureExtractor()
        self.df = None
        self.feature_matrix = None
    
    def fit(self, csv_path):
        """Load and prepare data"""
        self.df = pd.read_csv(csv_path)
        self.extractor.fit(self.df)
        
        # Extract features for all songs
        features = []
        for _, song in self.df.iterrows():
            f = self.extractor.extract_features(song)
            features.append(f)
        
        self.feature_matrix = np.array(features)
        print(f"✅ Trained on {len(self.df)} songs")
    
    def cosine_similarity(self, vec1, vec2):
        """Calculate similarity"""
        dot = np.dot(vec1, vec2)
        norm1 = np.linalg.norm(vec1)
        norm2 = np.linalg.norm(vec2)
        return dot / (norm1 * norm2) if norm1 > 0 and norm2 > 0 else 0
    
    def recommend(self, song_title, n=5):
        """Get recommendations for a song"""
        # Find song index
        song_mask = self.df['title'] == song_title
        if not song_mask.any():
            print(f"❌ Song '{song_title}' not found")
            return None
        
        song_idx = self.df[song_mask].index[0]
        target_features = self.feature_matrix[song_idx]
        
        # Calculate similarity with all songs
        similarities = []
        for idx in range(len(self.feature_matrix)):
            if idx != song_idx:  # Don't recommend the same song
                sim = self.cosine_similarity(target_features, self.feature_matrix[idx])
                similarities.append((idx, sim))
        
        # Sort and get top n
        similarities.sort(key=lambda x: x[1], reverse=True)
        top_indices = [idx for idx, _ in similarities[:n]]
        
        # Return as DataFrame
        recommendations = self.df.iloc[top_indices].copy()
        recommendations['similarity'] = [sim for _, sim in similarities[:n]]
        
        return recommendations[['title', 'artist', 'genre', 'bpm', 'similarity']]

# Task 2: Test recommender
recommender = SimpleRecommender()
recommender.fit('songs.csv')

print("\n🎵 Recommendations for 'Imagine':")
recs = recommender.recommend('Imagine', n=5)
print(recs)

# Task 3: Test with different songs
test_songs = ['Billie Jean', 'Sweet Child O Mine', 'Yesterday']
for song in test_songs:
    print(f"\n🎵 Recommendations for '{song}':")
    recs = recommender.recommend(song, n=3)
    if recs is not None:
        print(recs)
```

**✅ Success Criteria:**
- [ ] Recommender class works
- [ ] Gets reasonable recommendations
- [ ] Can explain why songs are similar
- [ ] Tested with 5+ different songs

---

### **Weekend - Days 6-7: Documentation & Review**

**Saturday (2h):**
```markdown
# Task 1: Write README.md for your project

# Song Recommender

## What it does
Recommends similar songs based on genre and BPM.

## How it works
1. Converts songs to feature vectors (one-hot genre + normalized BPM)
2. Calculates cosine similarity between songs
3. Returns most similar songs

## Usage
```python
recommender = SimpleRecommender()
recommender.fit('songs.csv')
recs = recommender.recommend('Imagine', n=5)
```

## What I learned
- Python basics (lists, dicts, functions)
- Pandas for data manipulation
- NumPy for numerical operations
- Feature engineering (one-hot encoding, normalization)
- Similarity metrics (cosine similarity)

## Next steps
- Add more features (artist, year)
- Use scikit-learn
- Create API
```

**Sunday (2h):**
```python
# Task 2: Improve your recommender

class ImprovedRecommender(SimpleRecommender):
    def recommend_diverse(self, song_title, n=5):
        """Get diverse recommendations (max 2 per genre)"""
        recs = self.recommend(song_title, n=n*2)  # Get more
        
        # Limit per genre
        diverse = recs.groupby('genre').head(2)
        return diverse.head(n)
    
    def explain_recommendation(self, song_title, recommended_title):
        """Explain why a song was recommended"""
        # Find both songs
        song1_idx = self.df[self.df['title'] == song_title].index[0]
        song2_idx = self.df[self.df['title'] == recommended_title].index[0]
        
        song1 = self.df.iloc[song1_idx]
        song2 = self.df.iloc[song2_idx]
        
        # Compare
        same_genre = song1['genre'] == song2['genre']
        bpm_diff = abs(song1['bpm'] - song2['bpm'])
        
        explanation = f"Similar because: "
        if same_genre:
            explanation += f"Same genre ({song1['genre']}) "
        explanation += f"and BPM difference is only {bpm_diff}"
        
        return explanation

# Test improvements
recommender = ImprovedRecommender()
recommender.fit('songs.csv')

print("🎯 Diverse recommendations:")
print(recommender.recommend_diverse('Imagine'))

print("\n📝 Explanation:")
recs = recommender.recommend('Imagine', n=1)
rec_title = recs.iloc[0]['title']
print(recommender.explain_recommendation('Imagine', rec_title))
```

**✅ Week 1 Complete!**
- [ ] Simple recommender working
- [ ] Tested with real songs
- [ ] Documentation written
- [ ] Learning journal updated
- [ ] Ready for Week 2 (scikit-learn)

---

## 📆 Week 2: scikit-learn & ML (Dec 23-29)

### **Monday - Day 8: scikit-learn Introduction**

**Time: 2-3 hours**

```bash
# Install scikit-learn
pip install scikit-learn
```

```python
# Task 1: Rewrite recommender with sklearn
from sklearn.neighbors import NearestNeighbors
from sklearn.preprocessing import StandardScaler
import pandas as pd
import numpy as np

class SklearnRecommender:
    def __init__(self, n_neighbors=5):
        self.n_neighbors = n_neighbors
        self.scaler = StandardScaler()
        self.knn = NearestNeighbors(
            n_neighbors=n_neighbors + 1,  # +1 for the song itself
            metric='cosine'
        )
        self.df = None
        self.feature_matrix = None
    
    def _extract_features(self, df):
        """Simple features: just BPM for now"""
        # We'll add genre encoding next
        features = df[['bpm']].values
        return features
    
    def fit(self, csv_path):
        """Train the recommender"""
        self.df = pd.read_csv(csv_path)
        
        # Extract features
        self.feature_matrix = self._extract_features(self.df)
        
        # Scale features (important!)
        self.feature_matrix = self.scaler.fit_transform(self.feature_matrix)
        
        # Fit KNN
        self.knn.fit(self.feature_matrix)
        
        print(f"✅ Trained on {len(self.df)} songs")
    
    def recommend(self, song_title):
        """Get recommendations"""
        # Find song
        song_mask = self.df['title'] == song_title
        if not song_mask.any():
            print(f"❌ Song not found")
            return None
        
        song_idx = self.df[song_mask].index[0]
        
        # Get neighbors
        distances, indices = self.knn.kneighbors(
            [self.feature_matrix[song_idx]]
        )
        
        # Remove first (the song itself)
        indices = indices[0][1:]
        distances = distances[0][1:]
        
        # Return recommendations
        recs = self.df.iloc[indices].copy()
        recs['similarity'] = 1 - distances  # Convert distance to similarity
        
        return recs[['title', 'artist', 'genre', 'bpm', 'similarity']]

# Test
recommender = SklearnRecommender()
recommender.fit('songs.csv')

print("🎵 Recommendations:")
print(recommender.recommend('Imagine'))
```

**✅ Success Criteria:**
- [ ] sklearn recommender works
- [ ] Understand fit-transform pattern
- [ ] Results similar to Week 1 version
- [ ] Code is simpler with sklearn

---

### **Tuesday - Day 9: Feature Engineering with sklearn**

```python
# Task: Add genre encoding using sklearn
from sklearn.preprocessing import OneHotEncoder
import pandas as pd
import numpy as np

class ImprovedSklearnRecommender(SklearnRecommender):
    def __init__(self, n_neighbors=5):
        super().__init__(n_neighbors)
        self.genre_encoder = OneHotEncoder(sparse_output=False)
    
    def _extract_features(self, df):
        """Extract genre + BPM features"""
        # Encode genres
        genres = df[['genre']].values
        genre_features = self.genre_encoder.fit_transform(genres)
        
        # Get BPM
        bpm_features = df[['bpm']].values
        
        # Combine
        features = np.concatenate([genre_features, bpm_features], axis=1)
        return features

# Test
recommender = ImprovedSklearnRecommender()
recommender.fit('songs.csv')

print("🎵 Recommendations with genre + BPM:")
print(recommender.recommend('Imagine'))
```

**✅ Success Criteria:**
- [ ] Genre encoding works
- [ ] Recommendations include genre similarity
- [ ] Results better than BPM-only version

---

### **Wednesday - Day 10: Evaluation Metrics**

```python
# Task: Implement evaluation
def precision_at_k(recommended, relevant, k=5):
    """
    Precision@K: How many recommendations are relevant?
    
    Example:
    recommended = ['song1', 'song2', 'song3', 'song4', 'song5']
    relevant = ['song2', 'song5', 'song8']
    precision@5 = 2/5 = 0.4
    """
    top_k = recommended[:k]
    relevant_in_top_k = len(set(top_k) & set(relevant))
    return relevant_in_top_k / k

# Manual test cases
test_cases = [
    {
        'song': 'Imagine',
        'expected_similar': ['Let It Be', 'Hey Jude', 'Yesterday']
    },
    {
        'song': 'Billie Jean',
        'expected_similar': ['Thriller', 'Beat It', 'Bad']
    }
]

recommender = ImprovedSklearnRecommender()
recommender.fit('songs.csv')

for test in test_cases:
    recs = recommender.recommend(test['song'])
    recommended_titles = recs['title'].tolist()
    
    precision = precision_at_k(recommended_titles, test['expected_similar'])
    print(f"Song: {test['song']}")
    print(f"Precision@5: {precision:.2f}")
    print(f"Recommended: {recommended_titles[:3]}")
    print()
```

**✅ Success Criteria:**
- [ ] Can calculate precision@k
- [ ] Created 10+ test cases
- [ ] Documented which work/don't work

---

### **Thursday - Day 11: Hyperparameter Tuning**

```python
# Task: Find optimal k (number of neighbors)
def evaluate_k(csv_path, k_values=[3, 5, 10, 15, 20]):
    """Test different k values"""
    results = []
    
    for k in k_values:
        recommender = ImprovedSklearnRecommender(n_neighbors=k)
        recommender.fit(csv_path)
        
        # Evaluate with test cases
        # (use your test cases from Day 10)
        avg_precision = 0.0  # Calculate from test cases
        
        results.append({
            'k': k,
            'precision': avg_precision
        })
        
        print(f"k={k}: precision={avg_precision:.3f}")
    
    return results

# Run experiment
results = evaluate_k('songs.csv')

# Find best k
best = max(results, key=lambda x: x['precision'])
print(f"\n✅ Best k: {best['k']}")
```

**✅ Success Criteria:**
- [ ] Tested k values from 3 to 20
- [ ] Found optimal k
- [ ] Understand trade-offs

---

### **Friday - Day 12: FastAPI Basics**

```bash
# Install FastAPI
pip install fastapi uvicorn
```

```python
# Task: Create first API (main.py)
from fastapi import FastAPI
from pydantic import BaseModel

app = FastAPI(title="Song Recommender API")

# Load recommender on startup
recommender = None

@app.on_event("startup")
async def startup():
    global recommender
    recommender = ImprovedSklearnRecommender()
    recommender.fit('songs.csv')
    print("✅ Recommender loaded")

# Health check
@app.get("/")
async def root():
    return {"status": "active", "service": "song-recommender"}

# Request model
class RecommendRequest(BaseModel):
    song_title: str
    n: int = 5

# Recommendation endpoint
@app.post("/recommend")
async def recommend(request: RecommendRequest):
    try:
        recs = recommender.recommend(request.song_title)
        
        # Convert to dict
        recs_list = recs.to_dict('records')
        
        return {
            "success": True,
            "data": {
                "query_song": request.song_title,
                "recommendations": recs_list
            }
        }
    except Exception as e:
        return {
            "success": False,
            "error": str(e)
        }

# Run with: uvicorn main:app --reload
```

**Test API:**
```bash
# Terminal 1: Start API
uvicorn main:app --reload

# Terminal 2: Test
curl -X POST http://localhost:8000/recommend \
  -H "Content-Type: application/json" \
  -d '{"song_title": "Imagine", "n": 5}'
```

**✅ Success Criteria:**
- [ ] FastAPI running locally
- [ ] Can call /recommend endpoint
- [ ] Returns JSON response
- [ ] Automatic docs at /docs

---

### **Weekend - Days 13-14: Integration with MAUI**

**Saturday:**
```csharp
// Task 1: Create C# service (copy from MAUI_INTEGRATION_GUIDE.md)
// File: Services/AI/Implementation/RecommendationService.cs

// Task 2: Register in MauiProgram.cs

// Task 3: Test with Postman first
```

**Sunday:**
```csharp
// Task 4: Create ViewModel
// File: ViewModels/AI/SongSelectionViewModel.cs

// Task 5: Wire up to existing page

// Task 6: End-to-end test
// - Start FastAPI
// - Start MAUI app
// - Click button
// - See recommendations!
```

**✅ Week 2 Complete!**
- [ ] sklearn recommender working
- [ ] FastAPI serving recommendations
- [ ] MAUI app consuming API
- [ ] End-to-end working!

---

## 🎯 Success Metrics

**After Week 1:**
- Can write Python confidently
- Understand pandas and numpy basics
- Built simple recommender from scratch

**After Week 2:**
- Know sklearn patterns
- Understand ML evaluation
- Have working API
- MAUI integration complete

**Ready for Month 2:**
- Audio processing (librosa)
- Pitch detection
- AWS deployment

---

## 📝 Daily Checklist Template

Copy this for each day:

```markdown
# Day X - [Date]

## Morning Standup (5 min)
- [ ] Review yesterday's work
- [ ] Read today's tasks
- [ ] Set 3 specific goals

## Implementation (2-3h)
- [ ] Task 1: [specific task]
- [ ] Task 2: [specific task]
- [ ] Task 3: [specific task]

## Testing (30 min)
- [ ] All code runs without errors
- [ ] Results make sense
- [ ] Edge cases handled

## Documentation (15 min)
- [ ] Code comments added
- [ ] Learning journal updated
- [ ] Questions noted

## Evening Review (5 min)
- [ ] All tasks completed?
- [ ] What went well?
- [ ] What was difficult?
- [ ] Ready for tomorrow?
```

---

## 🚨 When You Get Stuck

**20-Minute Rule:**
If stuck for >20 minutes, STOP and:

1. Write down the exact problem
2. What you've tried
3. Error messages (full text)
4. Post in learning journal
5. Take a break
6. Try fresh tomorrow OR ask for help

**You're learning, not competing!** Getting stuck is normal and valuable.

---

## 🎉 Completion Rewards

**Week 1 Done:**
- 🍕 Order your favorite food
- 📝 Write LinkedIn post about learning Python

**Week 2 Done:**
- 🎮 Take evening off
- 📧 Email a friend about your API

**Month 1 Done:**
- 🎊 Celebrate! You're an AI engineer now!
- 📄 Update resume
- 🚀 Start Month 2 (audio processing)

---

**Remember: Progress > Perfection! Let's build something amazing! 🚀**
