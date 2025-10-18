--
-- PostgreSQL database dump
--

\restrict qlpOQwhfydhHYnOvLdkwrocr3bNHHo1P6HgXuG6rkzOZaISI44GFjChhqlDmsa0

-- Dumped from database version 15.14 (Debian 15.14-1.pgdg13+1)
-- Dumped by pg_dump version 15.14 (Debian 15.14-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: media; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.media (
    id integer NOT NULL,
    name character varying(50),
    description text,
    release_date date,
    fsk integer,
    media_type_id integer
);


ALTER TABLE public.media OWNER TO postgres;

--
-- Name: Anime_AnimeID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Anime_AnimeID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Anime_AnimeID_seq" OWNER TO postgres;

--
-- Name: Anime_AnimeID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Anime_AnimeID_seq" OWNED BY public.media.id;


--
-- Name: rating; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rating (
    review_id integer NOT NULL,
    media_id integer NOT NULL,
    comment text,
    rating integer,
    username character varying
);


ALTER TABLE public.rating OWNER TO postgres;

--
-- Name: Review_ReviewID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Review_ReviewID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Review_ReviewID_seq" OWNER TO postgres;

--
-- Name: Review_ReviewID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Review_ReviewID_seq" OWNED BY public.rating.review_id;


--
-- Name: favourite; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favourite (
    media_id integer NOT NULL,
    username character varying NOT NULL
);


ALTER TABLE public.favourite OWNER TO postgres;

--
-- Name: genre; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre (
    id integer NOT NULL,
    name_eng character varying(50)
);


ALTER TABLE public.genre OWNER TO postgres;

--
-- Name: genre_genre_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.genre_genre_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.genre_genre_id_seq OWNER TO postgres;

--
-- Name: genre_genre_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.genre_genre_id_seq OWNED BY public.genre.id;


--
-- Name: genre_media; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre_media (
    media_id integer NOT NULL,
    genre_id integer NOT NULL
);


ALTER TABLE public.genre_media OWNER TO postgres;

--
-- Name: media_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.media_type (
    id integer NOT NULL,
    name character varying
);


ALTER TABLE public.media_type OWNER TO postgres;

--
-- Name: media_type_media_type_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.media_type_media_type_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.media_type_media_type_id_seq OWNER TO postgres;

--
-- Name: media_type_media_type_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.media_type_media_type_id_seq OWNED BY public.media_type.id;


--
-- Name: mrp_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.mrp_user (
    username character varying(20) NOT NULL,
    password text,
    email text,
    favorite_genre_id integer
);


ALTER TABLE public.mrp_user OWNER TO postgres;

--
-- Name: genre id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre ALTER COLUMN id SET DEFAULT nextval('public.genre_genre_id_seq'::regclass);


--
-- Name: media id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media ALTER COLUMN id SET DEFAULT nextval('public."Anime_AnimeID_seq"'::regclass);


--
-- Name: media_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media_type ALTER COLUMN id SET DEFAULT nextval('public.media_type_media_type_id_seq'::regclass);


--
-- Name: rating review_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating ALTER COLUMN review_id SET DEFAULT nextval('public."Review_ReviewID_seq"'::regclass);


--
-- Data for Name: favourite; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favourite (media_id, username) FROM stdin;
\.


--
-- Data for Name: genre; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.genre (id, name_eng) FROM stdin;
1	comedy
2	drama
\.


--
-- Data for Name: genre_media; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.genre_media (media_id, genre_id) FROM stdin;
4	1
4	2
\.


--
-- Data for Name: media; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.media (id, name, description, release_date, fsk, media_type_id) FROM stdin;
1	Naruto	\N	\N	\N	\N
4	Toradora	LOL	2025-09-22	8	1
\.


--
-- Data for Name: media_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.media_type (id, name) FROM stdin;
1	Movie
2	Series
3	Game
\.


--
-- Data for Name: mrp_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.mrp_user (username, password, email, favorite_genre_id) FROM stdin;
OBDAVID	12345	\N	\N
OBD	12345	\N	\N
OBDavid	5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5	\N	\N
Jürgen	299eff6446146b77cb344cd96e1361f6f77b4413e9f7e2e5da56c4e2a7b6e052	\N	\N
Jürgen2	299eff6446146b77cb344cd96e1361f6f77b4413e9f7e2e5da56c4e2a7b6e052	\N	\N
Jürgen3	299eff6446146b77cb344cd96e1361f6f77b4413e9f7e2e5da56c4e2a7b6e052	\N	\N
David	d9920b21023182434974f22dab64834d2b91181aedfb24c8959b93395d5eb19c	\N	\N
\.


--
-- Data for Name: rating; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rating (review_id, media_id, comment, rating, username) FROM stdin;
2	1	test	5	\N
\.


--
-- Name: Anime_AnimeID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Anime_AnimeID_seq"', 9, true);


--
-- Name: Review_ReviewID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Review_ReviewID_seq"', 2, true);


--
-- Name: genre_genre_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.genre_genre_id_seq', 2, true);


--
-- Name: media_type_media_type_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.media_type_media_type_id_seq', 3, true);


--
-- Name: favourite favourite_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourite
    ADD CONSTRAINT favourite_pkey PRIMARY KEY (username, media_id);


--
-- Name: genre_media genre_media_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_media
    ADD CONSTRAINT genre_media_pkey PRIMARY KEY (media_id, genre_id);


--
-- Name: genre genre_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre
    ADD CONSTRAINT genre_pkey PRIMARY KEY (id);


--
-- Name: media_type media_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media_type
    ADD CONSTRAINT media_type_pkey PRIMARY KEY (id);


--
-- Name: mrp_user mrp_user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mrp_user
    ADD CONSTRAINT mrp_user_pkey PRIMARY KEY (username);


--
-- Name: media pk_media; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media
    ADD CONSTRAINT pk_media PRIMARY KEY (id);


--
-- Name: rating pk_review; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating
    ADD CONSTRAINT pk_review PRIMARY KEY (review_id);


--
-- Name: rating rating; Type: CHECK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE public.rating
    ADD CONSTRAINT rating CHECK (((rating >= 1) AND (rating <= 5))) NOT VALID;


--
-- Name: mrp_user username; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mrp_user
    ADD CONSTRAINT username UNIQUE (username);


--
-- Name: rating FK_AnimeID; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating
    ADD CONSTRAINT "FK_AnimeID" FOREIGN KEY (media_id) REFERENCES public.media(id) NOT VALID;


--
-- Name: rating FK_Username; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating
    ADD CONSTRAINT "FK_Username" FOREIGN KEY (username) REFERENCES public.mrp_user(username) NOT VALID;


--
-- Name: mrp_user favorite_genre_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mrp_user
    ADD CONSTRAINT favorite_genre_id FOREIGN KEY (favorite_genre_id) REFERENCES public.genre(id) NOT VALID;


--
-- Name: genre_media fk_genre_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_media
    ADD CONSTRAINT fk_genre_id FOREIGN KEY (genre_id) REFERENCES public.genre(id);


--
-- Name: favourite fk_media_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourite
    ADD CONSTRAINT fk_media_id FOREIGN KEY (media_id) REFERENCES public.media(id);


--
-- Name: genre_media fk_media_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_media
    ADD CONSTRAINT fk_media_id FOREIGN KEY (media_id) REFERENCES public.media(id);


--
-- Name: media fk_media_type; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media
    ADD CONSTRAINT fk_media_type FOREIGN KEY (media_type_id) REFERENCES public.media_type(id) NOT VALID;


--
-- Name: favourite fk_username; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourite
    ADD CONSTRAINT fk_username FOREIGN KEY (username) REFERENCES public.mrp_user(username) NOT VALID;


--
-- PostgreSQL database dump complete
--

\unrestrict qlpOQwhfydhHYnOvLdkwrocr3bNHHo1P6HgXuG6rkzOZaISI44GFjChhqlDmsa0

