import streamlit as st
import pandas as pd
import numpy as np
from io import BytesIO
from datetime import datetime, timedelta

st.set_page_config(page_title="Admin Reports",
                   layout="centered", initial_sidebar_state="collapsed")

# css
st.markdown(
    """
    <style>
        /*hide streamlit options*/
        header[data-testid="stHeader"], #MainMenu, footer {
            display: none !important;
        }


        /*background*/
        [data-testid="stAppViewContainer"] {
            background-color: #F4F6FA !important;
        }

        /*header*/
        .custom-header {
            background-color: #1e3a8a;
            color: white;
            width: 100%;
            padding: 1rem 0;
            margin: 0;
            position: fixed; /* keeps it pinned at top */
            top: 0;
            left: 0;
            right: 0;
            z-index: 1000;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .header-inner {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 3rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .header-title {
            font-size: 1.75rem;
            font-weight: 550;
            line-height: 0.5;
        }

        .header-subtitle {
            font-size: 0.9rem;
            margin-top: 0.25rem;
            opacity: 0.9;
        }

        .nav-links {
            display: flex;
            gap: 2.5rem;
        }

        .nav-links a {
            color: white;
            text-decoration: none;
            font-size: 1.10rem;
            font-weight: 300;
        }

        /*buttons*/
        .stButton > button {
            background-color: #0B5FFF !important;
            color: white !important;
            border-radius: 6px !important;
            padding: 0.6rem 2.5rem !important;
            border: none !important;
            font-weight: 500 !important;
        }

        .stButton > button:hover {
            background-color: #004BCE !important;
        }

        .stDownloadButton > button {
            background-color: #28A745 !important;
            color: white !important;
            border-radius: 6px !important;
            padding: 0.6rem 2.5rem !important;
            border: none !important;
            font-weight: 500 !important;
        }

        .stDownloadButton > button:hover {
            background-color: #1E8E3E !important;
        }
    </style>
    """,
    unsafe_allow_html=True
)


# html for header
st.markdown("""
<div class="custom-header">
    <div class="header-inner">
        <div>
            <div class="header-title">University Lost & Found</div>
            <div class="header-subtitle">Student & Lecturer Portal</div>
        </div>
        <div class="nav-links">
            <a href="#">Home</a>
            <a href="#">Report Lost Item</a>
            <a href="#">Report Found Item</a>
            <a href="#">System Administrator</a>
        </div>
    </div>
</div>
<div class="main-content">
""", unsafe_allow_html=True)


# generate random lost and found values
def gen_ran_val(num_items: int = 300, year: int = datetime.now().year) -> pd.DataFrame:
    rng = np.random.default_rng(seed=123)
    categories = ["Electronics", "Clothing",
                  "Personal", "Miscellaneous", "Stationery"]
    statuses = ["Lost", "Found", "Claimed"]
    rows = []
    for i in range(1, num_items + 1):
        month = rng.integers(1, min(12, datetime.now().month) + 1)
        day = rng.integers(1, 28)
        dt = datetime(year, month, int(day), rng.integers(8, 18), 0, 0)
        status = rng.choice(statuses, p=[0.45, 0.45, 0.1])
        category = rng.choice(categories)
        rows.append({
            "ItemID": i,
            "UserID": rng.integers(1, 50),
            "Title": f"Item {i}",
            "Description": f"Mock description {i}",
            "Category": category,
            "Location": f"Location {rng.integers(1,10)}",
            "DateTime": dt,
            "Status": status
        })
    return pd.DataFrame(rows)


# generate random claims
def gen_ran_claims(items_df: pd.DataFrame) -> pd.DataFrame:
    rng = np.random.default_rng(seed=999)
    claims = []
    claimable_items = items_df.sample(frac=0.2, random_state=42)
    claim_id = 1
    for _, row in claimable_items.iterrows():
        n_claims = rng.integers(0, 3)
        for _ in range(n_claims):
            status = rng.choice(
                ["Pending", "Approved", "Rejected"], p=[0.5, 0.3, 0.2])
            claims.append({
                "ClaimID": claim_id,
                "ItemID": row["ItemID"],
                "UserID": rng.integers(1, 50),
                "Status": status,
                "CreatedBy": f"user{rng.integers(1,50)}",
                "CreatedDate": row["DateTime"] + pd.Timedelta(days=int(rng.integers(0, 10))),
                "Reason": "This is mine fr"
            })
            claim_id += 1
    return pd.DataFrame(claims)


# load fake data
@st.cache_data(ttl=300)
def load_data(real_db: bool = False):
    if real_db:
        raise NotImplementedError("put actual SQL queries here")
    items = gen_ran_val()
    claims = gen_ran_claims(items)
    return items, claims


# excel reporting
def to_excel_bytes(dfs: dict) -> bytes:
    buffer = BytesIO()
    with pd.ExcelWriter(buffer, engine="openpyxl") as writer:
        for sheet_name, df in dfs.items():
            df_to_save = df.copy()
            for col in df_to_save.select_dtypes(include=["datetime64[ns]"]).columns:
                df_to_save[col] = df_to_save[col].dt.strftime(
                    "%Y-%m-%d %H:%M:%S")
            df_to_save.to_excel(
                writer, sheet_name=sheet_name[:31], index=False)
    buffer.seek(0)
    return buffer.getvalue()


# prep data
def prep_export(report_type: str, items_filtered, claims_filtered):

    # get dict of sheet_name put in df to export
    if report_type == "All":
        sheets = {
            "Lost Items": items_filtered[items_filtered["Status"] == "Lost"].sort_values("DateTime"),
            "Found Items": items_filtered[items_filtered["Status"] == "Found"].sort_values("DateTime"),
            "Claims": claims_filtered.sort_values("CreatedDate")
        }
    elif report_type == "Lost":
        sheets = {
            "Lost Items": items_filtered[items_filtered["Status"] == "Lost"].sort_values("DateTime")}
    elif report_type == "Found":
        sheets = {
            "Found Items": items_filtered[items_filtered["Status"] == "Found"].sort_values("DateTime")}
    elif report_type == "Claims":
        sheets = {"Claims": claims_filtered.sort_values("CreatedDate")}
    else:
        sheets = {}
    return sheets


# UI
st.subheader("Admin Reports")
st.write("Select date range and type of report to pull")

# date picker
today = datetime.now().date()
default_start = today - timedelta(days=90)
col1, col2 = st.columns(2)
with col1:
    start_date = st.date_input("Start date", value=default_start)
with col2:
    end_date = st.date_input("End date", value=today)

# type of report box
report_type = st.selectbox("Type of report", options=[
                           "All", "Lost", "Found", "Claims"], index=0)


# load fake data
items_df, claims_df = load_data(real_db=False)

# filter for date range dataframe for items and claims
items_filtered = items_df[(items_df["DateTime"].dt.date >= start_date) & (
    items_df["DateTime"].dt.date <= end_date)]
claims_filtered = claims_df[(claims_df["CreatedDate"].dt.date >= start_date) & (
    claims_df["CreatedDate"].dt.date <= end_date)]

# make the file name todays date
today_str = datetime.now().strftime("%Y-%m-%d")
file_basename = f"LostAndFound_{report_type}_Report_{today_str}.xlsx"


# generate button
if st.button("Generate Report"):
    sheets = prep_export(report_type, items_filtered, claims_filtered)
    if not any(len(df) > 0 for df in sheets.values()):
        st.warning("No data for selected type and date range.")
    else:
        excel_bytes = to_excel_bytes(sheets)
        st.success(f"Prepared {file_basename} — click download below.")
        st.download_button(
            label="Download Excel",
            data=excel_bytes,
            file_name=file_basename,
            mime="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        )
