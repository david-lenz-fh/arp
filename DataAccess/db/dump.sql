--
-- PostgreSQL database dump
--

\restrict sqVyHGPKyQVYTI2hLreV6v5sGBuhbq4IrwMZOE3LXc3HHfpW3GcW0nUXOGxRXmc

-- Dumped from database version 16.10 (Debian 16.10-1.pgdg13+1)
-- Dumped by pg_dump version 16.10 (Debian 16.10-1.pgdg13+1)

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
    title character varying(50),
    description text,
    release_date date,
    fsk integer,
    media_type text,
    creator text NOT NULL
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


ALTER SEQUENCE public."Anime_AnimeID_seq" OWNER TO postgres;

--
-- Name: Anime_AnimeID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Anime_AnimeID_seq" OWNED BY public.media.id;


--
-- Name: rating; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rating (
    rating_id integer NOT NULL,
    media_id integer NOT NULL,
    comment text,
    stars integer NOT NULL,
    username character varying NOT NULL,
    confirmed boolean NOT NULL,
    "timestamp" timestamp without time zone
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


ALTER SEQUENCE public."Review_ReviewID_seq" OWNER TO postgres;

--
-- Name: Review_ReviewID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Review_ReviewID_seq" OWNED BY public.rating.rating_id;


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
    genre_name character varying(20) NOT NULL
);


ALTER TABLE public.genre OWNER TO postgres;

--
-- Name: genre_media; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.genre_media (
    media_id integer NOT NULL,
    genre_name character varying(20) NOT NULL
);


ALTER TABLE public.genre_media OWNER TO postgres;

--
-- Name: media_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.media_type (
    name character varying NOT NULL
);


ALTER TABLE public.media_type OWNER TO postgres;

--
-- Name: mrp_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.mrp_user (
    username character varying(20) NOT NULL,
    password text,
    email text,
    favourite_genre_name character varying(20),
    activity_points integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.mrp_user OWNER TO postgres;

--
-- Name: rating_likes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rating_likes (
    rating_id integer NOT NULL,
    username character varying NOT NULL
);


ALTER TABLE public.rating_likes OWNER TO postgres;

--
-- Name: media id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media ALTER COLUMN id SET DEFAULT nextval('public."Anime_AnimeID_seq"'::regclass);


--
-- Name: rating rating_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating ALTER COLUMN rating_id SET DEFAULT nextval('public."Review_ReviewID_seq"'::regclass);


--
-- Data for Name: favourite; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favourite (media_id, username) FROM stdin;
1	alice
4	alice
1	bob
2	carol
4	dave
\.


--
-- Data for Name: genre; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.genre (genre_name) FROM stdin;
Action
Drama
Fantasy
Comedy
Sci-Fi
\.


--
-- Data for Name: genre_media; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.genre_media (media_id, genre_name) FROM stdin;
1	Action
1	Sci-Fi
1	Fantasy
2	Drama
3	Comedy
4	Sci-Fi
4	Action
12	Comedy
12	Drama
\.


--
-- Data for Name: media; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.media (id, title, description, release_date, fsk, media_type, creator) FROM stdin;
2	Silent Tears	Dramatischer Film über Verlust und Neuanfang	2021-11-03	12	Movie	carol
3	Laugh Protocol	Absurde Comedy-Serie mit Tech-Satire	2022-06-20	6	Series	bob
4	Starfall Odyssey	Sci-Fi RPG im fernen Universum	2024-02-01	16	Game	dave
1	Neon Blades	Cyberpunk Anime über Megacities und Konzernkriege	2023-04-12	16	Series	alice
12	Fairy Tail	natsu feuer	2008-01-01	8	Series	alice
\.


--
-- Data for Name: media_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.media_type (name) FROM stdin;
Anime
Movie
Series
Game
\.


--
-- Data for Name: mrp_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.mrp_user (username, password, email, favourite_genre_name, activity_points) FROM stdin;
bob	81b637d8fcd2c6da6359e6963113a1170de795e4b725b84d1e0b4cfd9ec58ce9	bob@example.com	Action	75
carol	4c26d9074c27d89ede59270c0ac14b71e071b15239519f75474b2f3ba63481f5	carol@example.com	Drama	30
dave	61ea0803f8853523b777d414ace3130cd4d3f92de2cd7ff8695c337d79c2eeee	dave@example.com	Sci-Fi	55
alice	2bd806c97f0e00af1a1fc3328fa763a9269723c8db8fac4f93af71db186d6e90	L.pathgen@gmx.com	Comedy	130
David	a6b54c20a7b96eeac1a911e6da3124a560fe6dc042ebf270e3676e7095b95652	David.lenz@mail.com	Drama	3
\.


--
-- Data for Name: rating; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rating (rating_id, media_id, comment, stars, username, confirmed, "timestamp") FROM stdin;
1	1	Stylisch und brutal gut	5	bob	t	2025-12-24 12:27:31.443966
2	1	Coole Welt, Story etwas wirr	4	carol	t	2025-12-24 12:27:31.443966
4	3	Musste oft lachen	4	dave	f	2025-12-24 12:27:31.443966
6	1	Visuell beeindruckend, Soundtrack top	5	alice	t	2025-12-24 13:41:50.957941
7	1	Nicht ganz mein Stil, aber gut gemacht	3	dave	t	2025-12-24 13:41:50.957941
8	2	Solide Story, etwas vorhersehbar	3	bob	t	2025-12-24 13:41:50.957941
9	2	Sehr stark gespielt	4	dave	t	2025-12-24 13:41:50.957941
10	2	Hat mich echt mitgenommen	5	carol	t	2025-12-24 13:41:50.957941
11	3	Humor trifft nicht immer, aber kreativ	3	alice	f	2025-12-24 13:41:50.957941
12	3	Ein paar geniale Momente	4	bob	t	2025-12-24 13:41:50.957941
13	3	Mehr Satire als Comedy	2	carol	t	2025-12-24 13:41:50.957941
14	4	Atmosphäre großartig, Bugs nerven	3	bob	t	2025-12-24 13:41:50.957941
15	4	Richtig gutes Worldbuilding	5	carol	t	2025-12-24 13:41:50.957941
16	4	Eines der besten Sci-Fi Games	5	dave	t	2025-12-24 13:41:50.957941
5	4	Top Gameplay, Story ok	4	alice	f	2025-12-24 12:27:31.443966
17	12	perfekt	5	alice	t	2025-12-24 15:51:55.468974
18	1	ganz gut	4	David	t	2025-12-24 16:26:25.031382
\.


--
-- Data for Name: rating_likes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rating_likes (rating_id, username) FROM stdin;
1	alice
1	carol
5	dave
\.


--
-- Name: Anime_AnimeID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Anime_AnimeID_seq"', 13, true);


--
-- Name: Review_ReviewID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Review_ReviewID_seq"', 18, true);


--
-- Name: favourite favourite_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourite
    ADD CONSTRAINT favourite_pkey PRIMARY KEY (username, media_id);


--
-- Name: genre_media genre_media_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_media
    ADD CONSTRAINT genre_media_pkey PRIMARY KEY (media_id, genre_name);


--
-- Name: genre genre_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre
    ADD CONSTRAINT genre_pkey PRIMARY KEY (genre_name);


--
-- Name: media_type media_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media_type
    ADD CONSTRAINT media_type_pkey PRIMARY KEY (name);


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
    ADD CONSTRAINT pk_review PRIMARY KEY (rating_id);


--
-- Name: rating rating; Type: CHECK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE public.rating
    ADD CONSTRAINT rating CHECK (((stars >= 1) AND (stars <= 5))) NOT VALID;


--
-- Name: rating_likes rating_likes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating_likes
    ADD CONSTRAINT rating_likes_pkey PRIMARY KEY (rating_id, username);


--
-- Name: rating unique_user_media; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating
    ADD CONSTRAINT unique_user_media UNIQUE (media_id, username);


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
-- Name: media fk_creator; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.media
    ADD CONSTRAINT fk_creator FOREIGN KEY (creator) REFERENCES public.mrp_user(username) NOT VALID;


--
-- Name: mrp_user fk_favourite_genre_name; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mrp_user
    ADD CONSTRAINT fk_favourite_genre_name FOREIGN KEY (favourite_genre_name) REFERENCES public.genre(genre_name) NOT VALID;


--
-- Name: genre_media fk_genre_name; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.genre_media
    ADD CONSTRAINT fk_genre_name FOREIGN KEY (genre_name) REFERENCES public.genre(genre_name);


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
    ADD CONSTRAINT fk_media_type FOREIGN KEY (media_type) REFERENCES public.media_type(name) NOT VALID;


--
-- Name: rating_likes fk_rating_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating_likes
    ADD CONSTRAINT fk_rating_id FOREIGN KEY (rating_id) REFERENCES public.rating(rating_id);


--
-- Name: favourite fk_username; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourite
    ADD CONSTRAINT fk_username FOREIGN KEY (username) REFERENCES public.mrp_user(username) NOT VALID;


--
-- Name: rating_likes fk_username; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rating_likes
    ADD CONSTRAINT fk_username FOREIGN KEY (username) REFERENCES public.mrp_user(username);


--
-- PostgreSQL database dump complete
--

\unrestrict sqVyHGPKyQVYTI2hLreV6v5sGBuhbq4IrwMZOE3LXc3HHfpW3GcW0nUXOGxRXmc

